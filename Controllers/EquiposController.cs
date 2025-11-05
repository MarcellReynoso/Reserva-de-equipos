using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;
using System.Security.Claims;

namespace Reserva_de_equipos.Controllers
{
    public class EquiposController : Controller
    {
        private readonly DbReservaContext _context;
        public EquiposController(DbReservaContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var usuarioId)) return Forbid();

            var equipos = await _context.Equipos
                .AsNoTracking()
                .Include(e => e.TipoEquipo)
                .Where(e => e.UsuarioId == usuarioId)
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            ViewBag.TiposEquipo = new SelectList(
                await _context.TipoEquipos.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync(),
                "TipoEquipoId", "Nombre"
            );

            return View(equipos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Equipo model, IFormFile? imagen, [FromServices] IWebHostEnvironment env)
        {
            if (string.IsNullOrWhiteSpace(model?.Nombre))
            {
                TempData["Mensaje"] = "El nombre del equipo es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

            if (!model.FechaInicio.HasValue || !model.FechaFin.HasValue)
            {
                TempData["Mensaje"] = "Las fechas de inicio y fin son obligatorias.";
                return RedirectToAction(nameof(Index));
            }
            if (model.FechaInicio > model.FechaFin)
            {
                TempData["Mensaje"] = "La fecha de inicio no puede ser mayor que la fecha de fin.";
                return RedirectToAction(nameof(Index));
            }

            if (model.TipoEquipoId <= 0 || !await _context.TipoEquipos.AnyAsync(t => t.TipoEquipoId == model.TipoEquipoId))
            {
                TempData["Mensaje"] = "Debe seleccionar un tipo de equipo válido.";
                return RedirectToAction(nameof(Index));
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var usuarioId))
            {
                TempData["Mensaje"] = "No se pudo identificar al usuario.";
                return RedirectToAction(nameof(Index));
            }

            string? imagenUrl = null;
            try
            {
                if (imagen != null && imagen.Length > 0)
                {
                    var carpetaSubida = "assets/images/products";
                    var uploadsAbs = Path.Combine(env.WebRootPath, carpetaSubida);
                    if (!Directory.Exists(uploadsAbs)) Directory.CreateDirectory(uploadsAbs);

                    var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(imagen.FileName)}";
                    var pathAbs = Path.Combine(uploadsAbs, fileName);
                    using (var fs = new FileStream(pathAbs, FileMode.Create)) await imagen.CopyToAsync(fs);

                    imagenUrl = "/" + Path.Combine(carpetaSubida, fileName).Replace("\\", "/");
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Path.Combine(env.WebRootPath, "log.txt"),
                    $"{DateTime.Now:HH:mm:ss} - ERROR al guardar imagen: {ex.Message}\n");
            }

            var equipo = new Equipo
            {
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                TipoEquipoId = model.TipoEquipoId,
                FechaInicio = model.FechaInicio,
                FechaFin = model.FechaFin,
                ImagenUrl = imagenUrl,
                UsuarioId = usuarioId
            };

            using var tx = await _context.Database.BeginTransactionAsync();

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            var rolNombre = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (string.Equals(rolNombre, "Responsable", StringComparison.OrdinalIgnoreCase))
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
                if (usuario != null)
                {
                    usuario.EquipoId = equipo.EquipoId;
                    await _context.SaveChangesAsync();
                }
            }

            await tx.CommitAsync();

            TempData["Mensaje"] = "Equipo creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var equipo = await _context.Equipos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EquipoId == id);

            if (equipo == null)
                return NotFound();

            ViewBag.TiposEquipo = new SelectList(await _context.TipoEquipos.ToListAsync(), "TipoEquipoId", "Nombre", equipo.TipoEquipoId);

            return PartialView("_EditEquipo", equipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Equipo model, IFormFile? imagen, [FromServices] IWebHostEnvironment env)
        {
            if (model == null || model.EquipoId <= 0)
            {
                TempData["Mensaje"] = "Registro no válido.";
                return RedirectToAction(nameof(Index));
            }

            var entity = await _context.Equipos.FirstOrDefaultAsync(e => e.EquipoId == model.EquipoId);
            if (entity == null)
            {
                TempData["Mensaje"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var usuarioId))
            {
                TempData["Mensaje"] = "No se pudo identificar al usuario.";
                return RedirectToAction(nameof(Index));
            }

            if (entity.UsuarioId != usuarioId)
            {
                TempData["Mensaje"] = "No tiene permisos para editar este equipo.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                TempData["Mensaje"] = "El nombre del equipo es obligatorio.";
                return RedirectToAction(nameof(Index));
            }
            if (!model.FechaInicio.HasValue || !model.FechaFin.HasValue)
            {
                TempData["Mensaje"] = "Las fechas de inicio y fin son obligatorias.";
                return RedirectToAction(nameof(Index));
            }
            if (model.FechaInicio > model.FechaFin)
            {
                TempData["Mensaje"] = "La fecha de inicio no puede ser mayor que la fecha de fin.";
                return RedirectToAction(nameof(Index));
            }
            if (model.TipoEquipoId <= 0 || !await _context.TipoEquipos.AnyAsync(t => t.TipoEquipoId == model.TipoEquipoId))
            {
                TempData["Mensaje"] = "Debe seleccionar un tipo de equipo válido.";
                return RedirectToAction(nameof(Index));
            }

            if (imagen != null && imagen.Length > 0)
            {
                try
                {
                    var carpetaSubida = "assets/images/products";
                    var uploadsAbs = Path.Combine(env.WebRootPath, carpetaSubida);
                    if (!Directory.Exists(uploadsAbs)) Directory.CreateDirectory(uploadsAbs);

                    var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(imagen.FileName)}";
                    var pathAbs = Path.Combine(uploadsAbs, fileName);
                    using (var fs = new FileStream(pathAbs, FileMode.Create)) await imagen.CopyToAsync(fs);

                    if (!string.IsNullOrEmpty(entity.ImagenUrl))
                    {
                        var oldPath = Path.Combine(env.WebRootPath, entity.ImagenUrl.TrimStart('/').Replace("/", "\\"));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                    entity.ImagenUrl = "/" + Path.Combine(carpetaSubida, fileName).Replace("\\", "/");
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(Path.Combine(env.WebRootPath, "log.txt"),
                        $"{DateTime.Now:HH:mm:ss} - ERROR al actualizar imagen: {ex.Message}\n");
                }
            }

            entity.Nombre = model.Nombre;
            entity.Descripcion = model.Descripcion;
            entity.TipoEquipoId = model.TipoEquipoId;
            entity.FechaInicio = model.FechaInicio;
            entity.FechaFin = model.FechaFin;

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Equipo actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var entity = await _context.Equipos.FindAsync(id);
            if (entity != null)
            {
                _context.Equipos.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Equipo eliminado correctamente.";
            }
            else
            {
                TempData["Mensaje"] = "No se encontró el equipo.";
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Buscar(int? tipoId, int? responsableId)
        {
            var query = _context.Equipos
                .AsNoTracking()
                .Include(e => e.TipoEquipo)
                .OrderBy(e => e.Nombre)
                .AsQueryable();

            if (tipoId.HasValue && tipoId.Value > 0)
                query = query.Where(e => e.TipoEquipoId == tipoId.Value);

            if (responsableId.HasValue && responsableId.Value > 0)
                query = query.Where(e => e.UsuarioId == responsableId.Value);

            var modelos = await query.ToListAsync();

            ViewBag.TiposEquipo = new SelectList(
                await _context.TipoEquipos.OrderBy(t => t.Nombre).ToListAsync(),
                "TipoEquipoId", "Nombre", tipoId
            );

            ViewBag.Responsables = new SelectList(
                await _context.Usuarios
                    .Include(u => u.Rol)
                    .Where(u => u.Activo && u.EquipoId != null && u.Rol.Nombre == "Responsable")
                    .Select(u => new
                        {
                            u.UsuarioId,
                            Nombre = (u.Nombre + " " + (u.SegundoNombre ?? "") + " " + (u.ApellidoPaterno ?? "") + " " + (u.ApellidoMaterno ?? ""))
                        })
                    .OrderBy(r => r.Nombre)
                    .ToListAsync(),
                "UsuarioId", "Nombre", responsableId
            );

            return View(modelos);
        }


    }
}
