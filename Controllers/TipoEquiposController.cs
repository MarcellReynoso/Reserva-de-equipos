using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

namespace Reserva_de_equipos.Controllers
{
    public class TipoEquiposController : Controller
    {
        private readonly DbReservaContext _context;
        public TipoEquiposController(DbReservaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var tipos = await _context.TipoEquipos
                .OrderBy(t => t.Nombre)
                .ToListAsync();
            return View(tipos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoEquipo model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage
                            ?? "Complete los campos requeridos.";
                TempData["Mensaje"] = error;
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(TipoEquipo model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage
                            ?? "Complete los campos requeridos.";
                TempData["Mensaje"] = error;
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
    }
}
