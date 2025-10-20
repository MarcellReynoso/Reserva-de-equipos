using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

namespace Reserva_de_equipos.Controllers
{
    public class AreasController : Controller
    {
        private readonly DbReservaContext _context;

        public AreasController(DbReservaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var areas = _context.Areas
                .Include(a => a.Empresa);
            ViewBag.Empresas = await _context.Empresas
                .Select(e => new SelectListItem { Value = e.EmpresaId.ToString(), Text = e.Nombre })
                .ToListAsync();
            return View(await areas.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Area area)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(area);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Area creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Mensaje"] = "Error al crear el area. Por favor, intentelo nuevamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit (int? id)
        {
            if (id == null) return NotFound();
            ViewBag.Empresas = await _context.Empresas
                .Select(e => new SelectListItem { Value = e.EmpresaId.ToString(), Text = e.Nombre })
                .ToListAsync();

            var area = await _context.Areas
                .Where(a => a.AreaId == id)
                .FirstOrDefaultAsync();

            return PartialView("_EditArea", area);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Area area)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(area);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Area actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, Empresa empresa)
        {
            var area = await _context.Areas.FindAsync(id);
            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Area eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
