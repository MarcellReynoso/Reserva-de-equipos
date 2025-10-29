using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
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
                .Include(r => r.Rol)
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            var usuariosResponsables = await _context.Usuarios
                .Where(u => u.Activo && !_context.Responsables.Any(r => r.UsuarioId == u.UsuarioId))
                .OrderBy(u => u.Nombre)
                .Select(u => new SelectListItem 
                {
                    Value = u.UsuarioId.ToString(),
                    Text = (u.Nombre + " " + (u.SegundoNombre ?? "") + " " + u.ApellidoPaterno + " " + (u.ApellidoMaterno ?? "")).Replace("  ", " ").Trim()
                })
                .ToListAsync();

            ViewBag.UsuariosResponsables = usuariosResponsables;
            return View(responsables);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int usuarioId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
            if (usuario == null)
            {
                TempData["Mensaje"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            bool exists = await _context.Responsables.AnyAsync(r => r.UsuarioId == usuarioId);
            if (exists)
            {
                TempData["Mensaje"] = "Este usuario ya es un responsable.";
                return RedirectToAction(nameof(Index));
            }

            var rolResponsableId = await _context.Rols
                .Where(r => r.Nombre == "Responsable")
                .Select(r => r.RolId)
                .FirstOrDefaultAsync();
            if (rolResponsableId == 0)
            {
                TempData["Mensaje"] = "No se encontró el rol 'Responsable'.";
                return RedirectToAction(nameof(Index));
            }

            if (usuario.RolId != rolResponsableId)
            {
                usuario.RolId = rolResponsableId;
                await _context.SaveChangesAsync();
            }

            var responsable = new Responsable
            {
                UsuarioId = usuario.UsuarioId,
                RolId = rolResponsableId,
                Nombre = usuario.Nombre,
                SegundoNombre = usuario.SegundoNombre,
                ApellidoPaterno = usuario.ApellidoPaterno,
                ApellidoMaterno = usuario.ApellidoMaterno,
                Correo = usuario.Correo,
                Username = usuario.Username,
            };

            _context.Responsables.Add(responsable);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            TempData["Mensaje"] = "Responsable creado exitosamente y rol del usuario actualizado.";
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
            using var tx = await _context.Database.BeginTransactionAsync();

            var entity = await _context.Responsables
                .Include(r => r.Equipos)
                .FirstOrDefaultAsync(r => r.ResponsableId == id);

            if (entity == null)
            {
                TempData["Mensaje"] = "No se encontro el responsable.";
                return RedirectToAction(nameof(Index));
            }

            if (entity.Equipos.Any())
            {
                TempData["Mensaje"] = "El responsable tiene equipos asignados. No se puede eliminar.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == entity.UsuarioId);
            var rolClienteId = await _context.Rols
                .Where(r => r.Nombre == "Cliente")
                .Select(r => r.RolId)
                .FirstOrDefaultAsync();

            _context.Responsables.Remove(entity);

            if (usuario != null && rolClienteId != 0)
            {
                usuario.RolId = rolClienteId;
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            TempData["Mensaje"] = "Responsable eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
