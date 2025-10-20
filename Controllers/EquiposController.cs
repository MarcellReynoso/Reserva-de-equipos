using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

namespace Reserva_de_equipos.Controllers
{
    public class EquiposController : Controller
    {
        private readonly DbReservaContext _context;
        public EquiposController(DbReservaContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var equipos = await _context.Equipos
                .Include(e => e.Responsable)
                .Include(e => e.TipoEquipo)
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
        public async Task<IActionResult> Create(Equipo model)
        {

            if (string.IsNullOrWhiteSpace(model?.Nombre))
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

            var equipo = new Equipo
            {
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Disponible = true,
                ResponsableId = model.ResponsableId,
                TipoEquipoId = model.TipoEquipoId
            };

            _context.Add(equipo);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Equipo creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null) return NotFound();

            ViewBag.Responsables = await _context.Responsables
                .Select(r => new SelectListItem { Value = r.ResponsableId.ToString(), Text = r.Nombre })
                .OrderBy(r => r.Text)
                .ToListAsync();

            ViewBag.TiposEquipo = await _context.TipoEquipos
                .Select(t => new SelectListItem { Value = t.TipoEquipoId.ToString(), Text = t.Nombre })
                .OrderBy(t => t.Text)
                .ToListAsync();

            return PartialView("_EditEquipo", equipo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Equipo model)
        {
            if (model == null || model.EquipoId <= 0)
            {
                TempData["Mensaje"] = "Registro no válido.";
                return RedirectToAction(nameof(Index));
            }
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

            var entity = await _context.Equipos
                .FirstOrDefaultAsync(e => e.EquipoId == model.EquipoId);

            if (entity == null)
            {
                TempData["Mensaje"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            entity.Nombre = model.Nombre;
            entity.Descripcion = model.Descripcion;
            entity.ResponsableId = model.ResponsableId;
            entity.TipoEquipoId = model.TipoEquipoId;
            entity.Disponible = model.Disponible;

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
    }
}
