using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

namespace Reserva_de_equipos.Controllers
{
    public class ConductoresController : Controller
    {
        private readonly DbReservaContext _context;
        public ConductoresController(DbReservaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var conductores = await _context.Conductores
                .OrderBy(c => c.ApellidoPaterno).ThenBy(c => c.Nombre)
                .ToListAsync();
            return View(conductores);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Conductor model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Complete los campos requeridos.";
                TempData["Mensaje"] = error;
                return RedirectToAction(nameof(Index));
            }

            var conductor = new Conductor
            {
                Disponible = true,
                Nombre = model.Nombre,
                SegundoNombre = model.SegundoNombre,
                ApellidoPaterno = model.ApellidoPaterno,
                ApellidoMaterno = model.ApellidoMaterno
            };

            _context.Add(conductor);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Conductor creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var entity = await _context.Conductores.FindAsync(id);
            if (entity == null) return NotFound();
            return PartialView("_EditConductor", entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Conductor model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Complete los campos requeridos.";
                TempData["Mensaje"] = error;
                return RedirectToAction(nameof(Index));
            }

            var entity = await _context.Conductores.FirstOrDefaultAsync(c => c.ConductorId == model.ConductorId);
            if (entity == null)
            {
                TempData["Mensaje"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            entity.Nombre = model.Nombre;
            entity.SegundoNombre = model.SegundoNombre;
            entity.ApellidoPaterno = model.ApellidoPaterno;
            entity.ApellidoMaterno = model.ApellidoMaterno;
            entity.Disponible = model.Disponible;

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Conductor actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var entity = await _context.Conductores.FindAsync(id);
            if (entity != null)
            {
                _context.Conductores.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Conductor eliminado correctamente.";
            }
            else
            {
                TempData["Mensaje"] = "No se encontró el conductor.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
