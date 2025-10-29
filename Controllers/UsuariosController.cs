using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;
using Reserva_de_equipos.Models.ViewModels;
using Reserva_de_equipos.Utils;

namespace Reserva_de_equipos.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DbReservaContext _context;

        public UsuariosController(DbReservaContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = _context.Usuarios
                .Include(u => u.Area)
                .Include(u => u.Rol);

            //Obtener los roles y áreas de los usuarios
            ViewBag.Roles = await _context.Rols
                .Where(r => r.Nombre != "Responsable")
                .Select(r => new SelectListItem { Value = r.RolId.ToString(), Text = r.Nombre})
                .ToListAsync();
            
            ViewBag.Areas = await _context.Areas
                .Select(a => new SelectListItem { Value = a.AreaId.ToString(), Text = a.Nombre})
                .ToListAsync();

            return View(await usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Area)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Areas, "AreaId", "Nombre");
            ViewData["RolId"] = new SelectList(_context.Rols, "RolId", "Nombre");
            return View();
        }

        // POST: Usuarios/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values
                    .SelectMany(e => e.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Complete los campos requeridos. ";
                TempData["Mensaje"] = error;

                ViewData["AreaId"] = new SelectList(_context.Areas, "AreaId", "Nombre", model.AreaId);
                ViewData["RolId"] = new SelectList(_context.Rols, "RolId", "Nombre", model.RolId);

                return RedirectToAction(nameof(Index));
            }

            var responsableRolId = await GetResponsableRolId();
            if (model.RolId == responsableRolId && responsableRolId != 0)
            {
                TempData["Mensaje"] = "El rol 'Responsable' no se asigna desde Usuarios. Para ello vaya a la seccion Responsables.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                SegundoNombre = model.SegundoNombre,
                ApellidoPaterno = model.ApellidoPaterno,
                ApellidoMaterno = model.ApellidoMaterno,
                Correo = model.Correo,
                Password = Encrypt.GetSHA256(model.Password),
                Username = model.Username,
                Activo = true,
                RolId = model.RolId,
                AreaId = model.AreaId
            };

            await _context.AddAsync(usuario);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Usuario creado exitosamente";
            return RedirectToAction(nameof(Index));

        }

        // GET: Usuarios/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            //Obtener los roles y áreas de los usuarios
            ViewBag.Roles = await _context.Rols
                .Where(r => r.Nombre != "Responsable")
                .Select(r => new SelectListItem { Value = r.RolId.ToString(), Text = r.Nombre })
                .ToListAsync();
            ViewBag.Areas = await _context.Areas
                .Select(a => new SelectListItem{ Value = a.AreaId.ToString(),Text = a.Nombre })
                .ToListAsync();

            // Obtener el usuario a editar
            var usuario = await _context.Usuarios
                .Where(u => u.UsuarioId == id)
                .FirstOrDefaultAsync();
            if (usuario == null) return NotFound();

            var usuarioViewModel = new EditUsuarioViewModel
            {
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                SegundoNombre = usuario.SegundoNombre,
                ApellidoPaterno = usuario.ApellidoPaterno,
                ApellidoMaterno = usuario.ApellidoMaterno,
                Correo = usuario.Correo,
                Username = usuario.Username,
                Activo = usuario.Activo,
                RolId = usuario.RolId,
                AreaId = usuario.AreaId
            };

            return PartialView("_EditUsuario", usuarioViewModel);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUsuarioViewModel model)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.UsuarioId == model.UsuarioId)
                .FirstOrDefaultAsync();

            if (usuario == null) return NotFound();

            //Validación de contraseñas
            if (!string.IsNullOrWhiteSpace(model.Password) || !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                if (string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
                {
                    TempData["Mensaje"] = "Debe completar ambos campos de contraseña.";
                    return RedirectToAction("Index", "Usuarios");
                }

                if (model.Password != model.ConfirmPassword)
                {
                    TempData["Mensaje"] = "Las contraseñas no coinciden.";
                    return RedirectToAction("Index", "Usuarios");
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Revisa los campos requeridos.";
                return RedirectToAction("Index", "Usuarios");
            }

            var responsableRolId = await GetResponsableRolId();
            if (model.RolId == responsableRolId && responsableRolId != 0)
            {
                TempData["Mensaje"] = "El rol 'Responsable' no se asigna desde Usuarios. Para ello vaya a la seccion Responsables.";
                return RedirectToAction(nameof(Index));
            }

            usuario.Nombre = model.Nombre;
            usuario.SegundoNombre = model.SegundoNombre;
            usuario.ApellidoPaterno = model.ApellidoPaterno;
            usuario.ApellidoMaterno = model.ApellidoMaterno;
            usuario.Correo = model.Correo;
            usuario.Username = model.Username;
            // Solo actualizar la contraseña si el usuario ingresó una nueva
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                usuario.Password = Encrypt.GetSHA256(model.Password);
            }
            usuario.Activo = model.Activo;
            usuario.RolId = model.RolId;
            usuario.AreaId = model.AreaId;

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Usuario actualizado correctamente.";
            return RedirectToAction("Index", "Usuarios");
        }

        private async Task<int> GetResponsableRolId() =>
            await _context.Rols
                .Where(r => r.Nombre == "Responsable")
                .Select(r => r.RolId)
                .FirstOrDefaultAsync();


    }
}
