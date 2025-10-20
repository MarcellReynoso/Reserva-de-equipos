using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

namespace Reserva_de_equipos.Controllers
{
    public class AreaDisponiblesController : Controller
    {
        private readonly DbReservaContext _context;

        public AreaDisponiblesController(DbReservaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dbReservaContext = _context.AreaDisponibles
                .Include(a => a.Area)
                .Include(a => a.Equipo);
            ViewBag.Areas = await _context.Areas
                .Select(a => new SelectListItem { Value = a.AreaId.ToString(), Text = a.Nombre })
                .OrderBy(a => a.Text)
                .ToListAsync();
            ViewBag.Equipos = await _context.Equipos
                .Select(e => new SelectListItem { Value = e.EquipoId.ToString(), Text = e.Nombre })
                .OrderBy(e => e.Text)
                .ToListAsync();
            return View(await dbReservaContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AreaDisponible areaDisponible)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(areaDisponible);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Area disponible creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Mensaje"] = "Error al crear el area disponible. Por favor, intentelo nuevamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            ViewBag.Areas = await _context.Areas
                .Select(a => new SelectListItem { Value = a.AreaId.ToString(), Text = a.Nombre })
                .OrderBy(a => a.Text)
                .ToListAsync();
            ViewBag.Equipos = await _context.Equipos
                .Select(e => new SelectListItem { Value = e.EquipoId.ToString(), Text = e.Nombre })
                .OrderBy(e => e.Text)
                .ToListAsync();

            var areaDisponible = await _context.AreaDisponibles
                .Where(a => a.AreaDisponibleId == id)
                .FirstOrDefaultAsync();

            return PartialView("_EditAreaDisponible", areaDisponible);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AreaDisponible model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Complete los campos requeridos.";
                return RedirectToAction(nameof(Index));
            }

            var entity = await _context.AreaDisponibles
                .FirstOrDefaultAsync(a => a.AreaDisponibleId == model.AreaDisponibleId);
            if (entity == null)
            {
                TempData["Mensaje"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            entity.AreaId = model.AreaId;
            entity.EquipoId = model.EquipoId;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Area disponible editada exitosamente.";
            }
            catch (Exception)
            {
                TempData["Mensaje"] = "Error al editar el área disponible.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            var areaDisponible = await _context.AreaDisponibles.FindAsync(id);
            _context.AreaDisponibles.Remove(areaDisponible);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Area disponible eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
