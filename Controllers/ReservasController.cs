using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;
using Reserva_de_equipos.Models.ViewModels;
using System.Security.Claims;

namespace Reserva_de_equipos.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly DbReservaContext _context;

        public ReservasController(DbReservaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var reservas = await _context.Reservas
                .Include(r => r.Equipo)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
            return View(reservas);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? tipoEquipoId)
        {
            await ListEquipos(tipoEquipoId);
            return PartialView("_CreateReserva", new Reserva());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var nuevaReserva = new Reserva
                {
                    UsuarioId = usuarioId,
                    EquipoId = reserva.EquipoId,
                    FechaInicio = reserva.FechaInicio,
                    FechaFin = reserva.Indefinido ? null : reserva.FechaFin,
                    Indefinido = reserva.Indefinido,
                    Descripcion = reserva.Descripcion,
                    Ubicación = reserva.Ubicación,
                    Estado = "Pendiente",
                    Fecha = DateTime.Now
                };
                _context.Add(nuevaReserva);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Reserva creada exitosamente";
                return RedirectToAction("Index", "Calendario");
            }
            TempData["Mensaje"] = "Error al crear la reserva. Por favor, intentelo nuevamente.";
            return RedirectToAction("Index", "Calendario");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int? tipoEquipoId, bool readOnly = false)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Equipo)
                .FirstOrDefaultAsync(r => r.ReservaId == id);
            if (reserva == null) return NotFound();

            var tipo = tipoEquipoId ?? reserva.Equipo?.TipoEquipoId;
            await ListEquipos(tipo, reserva.EquipoId);

            ViewBag.ReadOnly = readOnly;
            return PartialView("_CreateReserva", reserva);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Reserva reserva)
        {
            if (id != reserva.ReservaId) return NotFound();

            var reservaExistente = await _context.Reservas
                .Where(r => r.ReservaId == id)
                .FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                try
                {
                    reservaExistente.FechaInicio = reserva.FechaInicio;
                    reservaExistente.FechaFin = reserva.Indefinido ? null : reserva.FechaFin;
                    reservaExistente.Indefinido = reserva.Indefinido;
                    reservaExistente.Descripcion = reserva.Descripcion;
                    reservaExistente.Ubicación = reserva.Ubicación;
                    reservaExistente.EquipoId = reserva.EquipoId;
                    _context.Update(reservaExistente);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Reserva actualizada correctamente.";

                    // 🔹 Redirige a la página anterior
                    var referer = Request.Headers["Referer"].ToString();
                    if (!string.IsNullOrEmpty(referer))
                        return Redirect(referer);

                    // Si por alguna razón no hay referer, vuelve al índice
                    return RedirectToAction("Index", "Reservas");
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Error de concurrencia en la base de datos.");
                }
            }
            await ListEquipos();
            return View(reservaExistente);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Equipo)
                .FirstOrDefaultAsync(r => r.ReservaId == id);

            if (reserva == null) return NotFound();

            await ListEquipos(reserva.Equipo?.TipoEquipoId);
            return PartialView("_ReadOnlyReserva", reserva);
        }


        private async Task ListEquipos(int? tipoEquipoId = null, int? equipoSeleccionadoId = null)
        {
            var tipos = await _context.TipoEquipos
                .Select(t => new SelectListItem { Value = t.TipoEquipoId.ToString(), Text = t.Nombre })
                .ToListAsync();
            ViewBag.TiposEquipo = new SelectList(tipos, "Value", "Text",
                tipoEquipoId.HasValue ? tipoEquipoId.Value.ToString() : null);

            var q = _context.Equipos.AsQueryable();

            if (tipoEquipoId.HasValue)
                q = q.Where(e => e.TipoEquipoId == tipoEquipoId);

            q = q.Where(e => e.Disponible);

            if (equipoSeleccionadoId.HasValue)
            {
                q = q.Union(
                        _context.Equipos
                            .Where(e => e.EquipoId == equipoSeleccionadoId.Value)
                    );
            }

            var lista = await q
                .Select(e => new SelectListItem { Value = e.EquipoId.ToString(), Text = e.Nombre })
                .ToListAsync();

            ViewBag.Equipos = new SelectList(lista, "Value", "Text",
                equipoSeleccionadoId.HasValue ? equipoSeleccionadoId.Value.ToString() : null);

            ViewBag.SelectedTipoEquipo = tipoEquipoId;
        }


        [HttpGet]
        public async Task<IActionResult> EquiposTipo(int tipoEquipoId)
        {
            var equipos = await _context.Equipos
                .Where(e => e.TipoEquipoId == tipoEquipoId && e.Disponible)
                .Select(e => new { equipoId = e.EquipoId, nombre = e.Nombre })
                .ToListAsync();
            return Json(equipos);
        }

        [HttpGet]
        public async Task<IActionResult> Pendientes()
        {
            var reservasPendientes = await _context.Reservas
                .Include(r => r.Equipo)
                .Where(r => r.Estado == "Pendiente")
                .OrderBy(r => r.Fecha)
                .ToListAsync();
            return View(reservasPendientes);
        }

        [HttpGet]
        public async Task<IActionResult> EnProceso()
        {
            var reservasEnProceso = await _context.Reservas
                .Include(r => r.Equipo)
                .Where(r => r.Estado == "En proceso")
                .OrderBy(r => r.Fecha)
                .ToListAsync();
            return View(reservasEnProceso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnProceso(int? id)
        {
            if (id is null) return BadRequest();
            var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.ReservaId == id);
            if (reserva is null) return NotFound();

            reserva.Estado = "En proceso";
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Reserva en proceso.";
            return RedirectToAction(nameof(Pendientes));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int? id)
        {
            var reserva = await _context.Reservas
                .Where(r => r.ReservaId == id)
                .FirstOrDefaultAsync();
            if (id != reserva.ReservaId) return NotFound();

            try
            {
                reserva.Estado = "Rechazado";
                _context.Update(reserva);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Reserva rechazada correctamente.";
                return RedirectToAction(nameof(Pendientes));
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Mensaje"] = "Ocurrió un problema al aprobar. Inténtalo nuevamente.";
                return RedirectToAction(nameof(Pendientes));
            }

        }

        [HttpGet]
        public async Task<IActionResult> AsignarConductor (int? id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Equipo)
                .Where(r => r.ReservaId == id)
                .FirstOrDefaultAsync();

            var conductores = await _context.Conductores
                .Where(c => c.Disponible)
                .Select(c => new SelectListItem { Value=c.ConductorId.ToString(), Text = $"{c.Nombre} {c.ApellidoPaterno}"})
                .ToListAsync();

            ViewBag.Conductores = conductores;

            var model = new AsignarConductorViewModel
            {
                ReservaId = reserva.ReservaId,
                ConductorId = reserva.ConductorId ?? 0,
            };

            return PartialView("_AsignarConductor", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarConductor (AsignarConductorViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.ReservaId == model.ReservaId);
            if (reserva == null) return NotFound();

            try
            {
                reserva.ConductorId = model.ConductorId;
                reserva.Estado = "Aprobado";
                _context.Update(reserva);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Conductor asignado correctamente.";
                return RedirectToAction(nameof(EnProceso));
            }
            catch (Exception ex)
            {
                {
                    TempData["Mensaje"] = "Ocurrió un problema al aprobar. Inténtalo nuevamente.";
                    Console.Write(ex.ToString());
                    return RedirectToAction(nameof(Pendientes));
                }
            }
        } 
    }
} 
