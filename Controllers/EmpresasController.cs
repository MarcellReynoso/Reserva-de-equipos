using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;

namespace Reserva_de_equipos.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly DbReservaContext _context;
        public EmpresasController(DbReservaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var empresas = await _context.Empresas
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return View(empresas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empresa model)
        {
            if (string.IsNullOrWhiteSpace(model?.Nombre))
            {
                TempData["Mensaje"] = "El nombre de la empresa es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Empresa creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return NotFound();
            return PartialView("_EditEmpresa", empresa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Empresa model)
        {
            if (model == null || model.EmpresaId <= 0)
            {
                TempData["Mensaje"] = "Registro no válido.";
                return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                TempData["Mensaje"] = "El nombre de la empresa es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

            var entity = await _context.Empresas.FirstOrDefaultAsync(e => e.EmpresaId == model.EmpresaId);
            if (entity == null)
            {
                TempData["Mensaje"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            entity.Nombre = model.Nombre;
            entity.Descripción = model.Descripción;

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Empresa actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var entity = await _context.Empresas.FindAsync(id);
            if (entity != null)
            {
                _context.Empresas.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Empresa eliminada correctamente.";
            }
            else
            {
                TempData["Mensaje"] = "No se encontró la empresa.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
