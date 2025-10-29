using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_de_equipos.Models;
using Reserva_de_equipos.Models.ViewModels;
using Reserva_de_equipos.Utils;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
            if (!ModelState.IsValid)
                return View("Index");

            var passwordHashed = Encrypt.GetSHA256(loginViewModel.Password);
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == loginViewModel.Username);

            // Usuario no existe
            if (user == null)
            {
                ViewData["Mensaje"] = "El usuario no existe.";
                return View("Index");
            }

            // Usuario inactivo
            if (!user.Activo)
            {
                ViewData["Mensaje"] = "Usuario inactivo.";
                return View("Index");
            }

            // Contraseña incorrecta
            if (!string.Equals(user.Password, passwordHashed, StringComparison.Ordinal))
            {
                ViewData["Mensaje"] = "Contraseña inválida.";
                return View("Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, user.Nombre ?? user.Username),
                new Claim("NombreApellido", $"{user.Nombre} {user.ApellidoPaterno}".Trim()),
                new Claim(ClaimTypes.Role, user.Rol?.Nombre ?? "Cliente"),
                new Claim("NombreCompleto", $"{user.Nombre} {user.SegundoNombre} {user.ApellidoPaterno} {user.ApellidoMaterno}".Replace("  "," ").Trim()),
                new Claim("IsActive", "true"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { AllowRefresh = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Index", "Home");
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
            usuario.Password = Encrypt.GetSHA256(Password);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Contrasena actualizada correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}
