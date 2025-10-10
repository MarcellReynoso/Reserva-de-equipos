using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;
using Reserva_de_equipos.Models.ViewModels;
using System.Security.Claims;

namespace Reserva_de_equipos.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly DbReservaContext _context;
        private readonly ILogger<AutenticacionController> _logger;

        public AutenticacionController(DbReservaContext context, ILogger<AutenticacionController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Usuarios
                    .Where(u => u.Username == loginViewModel.Username && u.Password == loginViewModel.Password)
                    .FirstOrDefaultAsync();
             
                if (user == null)
                {
                    ViewData["Mensaje"] = "Usuario o contraseña inválida.";
                    return View("Index");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, user.Nombre),
                    new Claim("NombreApellido", user.Nombre + " " + user.ApellidoPaterno),
                    new Claim(ClaimTypes.Role, _context.Rols.Find(user.RolId).Nombre),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                };

                await HttpContext.SignInAsync
                (
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                return RedirectToAction("Index", "Home");
            }
            return View("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string Password, string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                TempData["Mensaje"] = "Debe completar ambos campos.";
                return RedirectToAction("Index", "Home");
            }

            if (!Password.Equals(ConfirmPassword))
            {
                TempData["Mensaje"] = "Las contrasenas no coinciden.";
                return RedirectToAction("Index", "Home");
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim)) return Unauthorized();
            if (!int.TryParse(idClaim, out var usuarioId)) return Unauthorized();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
            if (usuario == null) return NotFound();

            usuario.Password = Password;
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Contrasena actualizada correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}
