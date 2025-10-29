using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

namespace Reserva_de_equipos.Controllers
{
    public class TipoEquiposController : Controller
    {
        private readonly DbReservaContext _context;
        private readonly IWebHostEnvironment _env;
        private const string IconFolderRel = "assets/images/icons";
        private static readonly string[] AllowedExt = [".png", ".jpg", ".jpeg", ".svg"];
        private const long MaxIconSizeBytes = 512 * 1024; // 512 KB

        public TipoEquiposController(DbReservaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var tipos = await _context.TipoEquipos
                .OrderBy(t => t.Nombre)
                .ToListAsync();
            return View(tipos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoEquipo model, IFormFile? icon)
        {
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage
                                      ?? "Complete los campos requeridos.";
                return RedirectToAction(nameof(Index));
            }

            if (icon != null && icon.Length > 0)
            {
                // Validacion icono
                var ext = Path.GetExtension(icon.FileName).ToLowerInvariant();
                if (!icon.ContentType.StartsWith("image/") || !AllowedExt.Contains(ext))
                {
                    TempData["Mensaje"] = "El icono debe ser una imagen (PNG/JPG/SVG).";
                    return RedirectToAction(nameof(Index));
                }
                if (icon.Length > MaxIconSizeBytes)
                {
                    TempData["Mensaje"] = "El icono no debe superar 512 KB.";
                    return RedirectToAction(nameof(Index));
                }

                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, IconFolderRel));

                var fileName = $"te_{Guid.NewGuid():N}{ext}";
                var (physical, web) = BuildIconPaths(fileName);

                using (var fs = System.IO.File.Create(physical))
                    await icon.CopyToAsync(fs);

                model.IconoUrl = web; // "~/assets/images/icons/te_....svg"
            }

            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Tipo de equipo creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tipo = await _context.TipoEquipos.FindAsync(id);
            if (tipo == null) return NotFound();
            return PartialView("_EditTipoEquipo", tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TipoEquipo model, IFormFile? icon)
        {
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage
                                      ?? "Complete los campos requeridos.";
                return RedirectToAction(nameof(Index));
            }

            var entity = await _context.TipoEquipos.FirstOrDefaultAsync(t => t.TipoEquipoId == model.TipoEquipoId);
            if (entity == null)
            {
                TempData["Mensaje"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            entity.Nombre = model.Nombre;
            entity.Descripcion = model.Descripcion;

            // Reemplazo de ícono si suben uno nuevo
            if (icon != null && icon.Length > 0)
            {
                var ext = Path.GetExtension(icon.FileName).ToLowerInvariant();
                if (!icon.ContentType.StartsWith("image/") || !AllowedExt.Contains(ext))
                {
                    TempData["Mensaje"] = "El archivo de ícono debe ser PNG/JPG/SVG.";
                    return RedirectToAction(nameof(Index));
                }
                if (icon.Length > MaxIconSizeBytes)
                {
                    TempData["Mensaje"] = "El icono no debe superar 512 KB.";
                    return RedirectToAction(nameof(Index));
                }

                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, IconFolderRel));

                var fileName = $"te_{Guid.NewGuid():N}{ext}";
                var (physical, web) = BuildIconPaths(fileName);

                using (var fs = System.IO.File.Create(physical))
                    await icon.CopyToAsync(fs);

                // Borrar anterior si existe
                if (!string.IsNullOrWhiteSpace(entity.IconoUrl))
                {
                    var prevRel = entity.IconoUrl.TrimStart('~', '/')
                                                 .Replace("/", Path.DirectorySeparatorChar.ToString());
                    var prevAbs = Path.Combine(_env.WebRootPath, prevRel);
                    if (System.IO.File.Exists(prevAbs)) System.IO.File.Delete(prevAbs);
                }

                entity.IconoUrl = web; // "~/assets/images/icons/..."
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Tipo de equipo actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var entity = await _context.TipoEquipos.FindAsync(id);
            if (entity != null)
            {
                // Remover archivo físico si hay URL
                if (!string.IsNullOrWhiteSpace(entity.IconoUrl))
                {
                    var rel = entity.IconoUrl.TrimStart('~', '/')
                                             .Replace("/", Path.DirectorySeparatorChar.ToString());
                    var abs = Path.Combine(_env.WebRootPath, rel);
                    if (System.IO.File.Exists(abs)) System.IO.File.Delete(abs);
                }

                _context.TipoEquipos.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Tipo de equipo eliminado correctamente.";
            }
            else
            {
                TempData["Mensaje"] = "No se encontró el tipo de equipo.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private (string physical, string web) BuildIconPaths(string fileName)
        {
            var physical = Path.Combine(_env.WebRootPath, IconFolderRel, fileName);
            var web = "~/" + Path.Combine(IconFolderRel, fileName).Replace("\\", "/");
            return (physical, web);
        }
    }
}
