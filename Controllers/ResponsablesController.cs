using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

namespace Reserva_de_equipos.Controllers
{
    public class ResponsablesController : Controller
    {
        private readonly DbReservaContext _context;
        public ResponsablesController(DbReservaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var responsables = await _context.Responsables
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return View(responsables);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Responsable model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Complete los campos requeridos.";
                TempData["Mensaje"] = error;
                return RedirectToAction(nameof(Index));
            }

            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Responsable creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var responsable = await _context.Responsables.FindAsync(id);
            if (responsable == null) return NotFound();
            return PartialView("_EditResponsable", responsable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Responsable model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Complete los campos requeridos.";
                TempData["Mensaje"] = error;
                return RedirectToAction(nameof(Index));
            }

            var entity = await _context.Responsables
                .FirstOrDefaultAsync(r => r.ResponsableId == model.ResponsableId);
            if (entity == null)
            {
                TempData["Mensaje"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            entity.Nombre = model.Nombre;
            entity.SegundoNombre = model.SegundoNombre;
            entity.ApellidoPaterno = model.ApellidoPaterno;
            entity.ApellidoMaterno = model.ApellidoMaterno;

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Responsable actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var entity = await _context.Responsables.FindAsync(id);
            if (entity != null)
            {
                _context.Responsables.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Responsable eliminado correctamente.";
            }
            else
            {
                TempData["Mensaje"] = "No se encontro el responsable.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
