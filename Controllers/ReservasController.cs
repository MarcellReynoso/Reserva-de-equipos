using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

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
            var reservas = await _context.Reservas
                .Include(r => r.Equipo)
                .ToListAsync();
            return View(reservas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Reserva creada exitosamente";
                return RedirectToAction("Index", "Calendario");
            }
            return RedirectToAction("Index", "Calendario");
        }

        [HttpGet]
        public async Task<IActionResult> Edit (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var reservas = await _context.Reservas
                .Where(r => r.ReservaId == id)
                .Include(r => r.Equipo)
                .FirstOrDefaultAsync();

            await ListEquipos();
            return PartialView("_CreateReserva", reservas);
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

        private async Task ListEquipos()
        {
            ViewBag.Equipos = await _context.Equipos
                .Select(e => new SelectListItem
                {
                    Value = e.EquipoId.ToString(),
                    Text = e.Nombre
                })
                .ToListAsync();
        }
    }
}
