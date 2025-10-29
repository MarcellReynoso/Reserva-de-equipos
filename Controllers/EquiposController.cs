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
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var responsable = await _context.Responsables
                .FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);

            var equipos = await _context.Equipos
                .Include(e => e.Responsable)
                .Include(e => e.TipoEquipo)
                .Where(e => e.ResponsableId == responsable.ResponsableId)
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            ViewBag.Responsables = await _context.Responsables
                .Select(r => new SelectListItem { Value = r.ResponsableId.ToString(), Text = r.Nombre })
                .OrderBy(r => r.Text)
                .ToListAsync();

            ViewBag.TiposEquipo = await _context.TipoEquipos
                .Select(t => new SelectListItem { Value = t.TipoEquipoId.ToString(), Text = t.Nombre })
                .OrderBy(t => t.Text)
                .ToListAsync();

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

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var responsable = await _context.Responsables
                .FirstOrDefaultAsync(r => r.ResponsableId == usuarioId);

            if (model.TipoEquipoId <= 0 || !await _context.TipoEquipos.AnyAsync(t => t.TipoEquipoId == model.TipoEquipoId))
            {
                TempData["Mensaje"] = "Debe seleccionar un tipo de equipo válido.";
                return RedirectToAction(nameof(Index));
            }

            if (model.FechaInicio.HasValue && model.FechaFin.HasValue && model.FechaInicio > model.FechaFin)
            {
                TempData["Mensaje"] = "La fecha de inicio no puede ser mayor que la fecha de fin.";
                return RedirectToAction(nameof(Index));
            }

            string? imagenUrl = null;

            try
            {
                if (imagen != null && imagen.Length > 0)
                {
                    var carpetaSubida = "assets/images/products";
                    var uploadsAbs = Path.Combine(env.WebRootPath, carpetaSubida);
                    if (!Directory.Exists(uploadsAbs))
                        Directory.CreateDirectory(uploadsAbs);

                    var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(imagen.FileName)}";
                    var pathAbs = Path.Combine(uploadsAbs, fileName);

                    using (var fs = new FileStream(pathAbs, FileMode.Create))
                        await imagen.CopyToAsync(fs);

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
                ResponsableId = responsable.ResponsableId,
                TipoEquipoId = model.TipoEquipoId,
                FechaInicio = model.FechaInicio,
                FechaFin = model.FechaFin,
                ImagenUrl = imagenUrl
            };

            _context.Add(equipo);
            await _context.SaveChangesAsync();

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
            ViewBag.Responsables = new SelectList(await _context.Responsables.ToListAsync(), "ResponsableId", "NombreCompleto", equipo.ResponsableId);

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

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var responsable = await _context.Responsables
                .FirstOrDefaultAsync(r => r.ResponsableId == usuarioId);

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                TempData["Mensaje"] = "El nombre del equipo es obligatorio.";
                return RedirectToAction(nameof(Index));
            }
            if (model.ResponsableId <= 0 || !await _context.Responsables.AnyAsync(r => r.ResponsableId == model.ResponsableId))
            {
                TempData["Mensaje"] = "Debe seleccionar un responsable válido.";
                return RedirectToAction(nameof(Index));
            }
            if (model.TipoEquipoId <= 0 || !await _context.TipoEquipos.AnyAsync(t => t.TipoEquipoId == model.TipoEquipoId))
            {
                TempData["Mensaje"] = "Debe seleccionar un tipo de equipo válido.";
                return RedirectToAction(nameof(Index));
            }

            // Imagen nueva
            if (imagen != null && imagen.Length > 0)
            {
                try
                {
                    var carpetaSubida = "assets/images/products";
                    var uploadsAbs = Path.Combine(env.WebRootPath, carpetaSubida);
                    if (!Directory.Exists(uploadsAbs))
                        Directory.CreateDirectory(uploadsAbs);

                    var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(imagen.FileName)}";
                    var pathAbs = Path.Combine(uploadsAbs, fileName);

                    using (var fs = new FileStream(pathAbs, FileMode.Create))
                        await imagen.CopyToAsync(fs);

                    // Eliminar imagen anterior si existía
                    if (!string.IsNullOrEmpty(entity.ImagenUrl))
                    {
                        var oldPath = Path.Combine(env.WebRootPath, entity.ImagenUrl.TrimStart('/').Replace("/", "\\"));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    entity.ImagenUrl = "/" + Path.Combine(carpetaSubida, fileName).Replace("\\", "/");
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(Path.Combine(env.WebRootPath, "log.txt"),
                        $"{DateTime.Now:HH:mm:ss} - ERROR al actualizar imagen: {ex.Message}\n");
                }
            }

            // Actualizar demás campos
            entity.Nombre = model.Nombre;
            entity.Descripcion = model.Descripcion;
            entity.ResponsableId = responsable.ResponsableId;
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
                .Include(e => e.Responsable)
                .OrderBy(e => e.Nombre)
                .AsQueryable();

            if (tipoId.HasValue && tipoId.Value > 0)
                query = query.Where(e => e.TipoEquipoId == tipoId.Value);

            if (responsableId.HasValue && responsableId.Value > 0)
                query = query.Where(e => e.ResponsableId == responsableId.Value);

            var modelos = await query.ToListAsync();

            ViewBag.TiposEquipo = new SelectList(
                await _context.TipoEquipos.OrderBy(t => t.Nombre).ToListAsync(),
                "TipoEquipoId", "Nombre", tipoId
            );

            ViewBag.Responsables = new SelectList(
                await _context.Responsables.OrderBy(r => r.Nombre).ToListAsync(),
                "ResponsableId", "Nombre", responsableId
            );

            return View(modelos);
        }


    }
}
