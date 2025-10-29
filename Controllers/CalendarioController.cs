using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Reserva_de_equipos.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reserva_de_equipos.Controllers
{
    [Authorize]
    [Route("Calendario")]
    public class CalendarioController : Controller
    {
        private readonly DbReservaContext _context;
        private readonly IConfiguration _configuration;

        public CalendarioController(DbReservaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET Calendario/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Index(int? id)
        {
            var equipo = await _context.Equipos
                .AsNoTracking()
                .Include(e => e.TipoEquipo)
                .FirstOrDefaultAsync(e => e.EquipoId == id);
            if (equipo == null) return NotFound();

            var iconoTipo = string.IsNullOrWhiteSpace(equipo.TipoEquipo?.IconoUrl)
                ? "~/assets/images/icono-placeholder.svg"
                : equipo.TipoEquipo!.IconoUrl;

            // Reservas solo de ese equipo
            var reservas = await _context.Reservas
                .AsNoTracking()
                .Include(r => r.usuario)
                .Where(r => r.EquipoId == id)
                .Select(r => new
                {
                    id = r.ReservaId,
                    start = r.FechaInicio,
                    end = r.Indefinido ? (DateTime?)null : r.FechaFin,
                    title = (r.usuario.Nombre + " " + r.usuario.ApellidoPaterno).Trim(),
                    estado = r.Estado,
                    descripcion = r.Descripcion,
                    ubicacion = r.Ubicación
                })
                .ToListAsync();

            ViewBag.Reservas = JsonConvert.SerializeObject(reservas);
            ViewBag.EquipoId = equipo.EquipoId;
            ViewBag.EquipoNombre = equipo.Nombre;
            ViewBag.EquipoTipo = equipo.TipoEquipo?.Nombre;
            ViewBag.EquipoImagenUrl = string.IsNullOrWhiteSpace(equipo.ImagenUrl)
                ? "/assets/images/placeholder.png"
                : equipo.ImagenUrl;
            ViewBag.IconoTipoEquipo = Url.Content(iconoTipo);

            var ubi = (string?)Request.Query["ubi"];
            var lat = (string?)Request.Query["lat"];
            var lon = (string?)Request.Query["lon"];

            ViewBag.Ubicacion = ubi;   // texto legible (display_name)
            ViewBag.Latitud = lat;
            ViewBag.Longitud = lon;

            return View("Index");
        }

        // POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int equipoId, DateTime fechaInicio, DateTime? fechaFin, bool indefinido, string? ubicacion, string? descripcion)
        {
            var ubi = Request.Form["ubicacion"];
            var lat = Request.Query["lat"];
            var lon = Request.Query["lon"];

            if (!indefinido)
            {
                if (!fechaFin.HasValue)
                {
                    TempData["Mensaje"] = "Debes ingresar fecha de fin o marcar indefinido.";
                    return RedirectToAction(nameof(Index), new { id = equipoId, ubi, lat, lon });
                }

                if (fechaFin.Value <= fechaInicio)
                {
                    TempData["Mensaje"] = "La fecha de fin debe ser mayor a la fecha de inicio.";
                    return RedirectToAction(nameof(Index), new { id = equipoId, ubi, lat, lon });
                }
            }

            // Validación de solapamiento (versión simplificada)
            var startDate = fechaInicio.Date;
            bool reservaOcupada = await _context.Reservas
                .AsNoTracking()
                .Where(r => r.EquipoId == equipoId)
                .AnyAsync(r =>
                    // 1) Nueva definida vs definida existente (inclusive en bordes)
                    (!indefinido && !r.Indefinido &&
                     fechaInicio <= r.FechaFin && fechaFin >= r.FechaInicio)

                    ||

                    // 2) Nueva definida vs existente indefinida (ocupa todo el día de su inicio)
                    (!indefinido && r.Indefinido &&
                     fechaInicio.Date <= r.FechaInicio.Date && fechaFin.HasValue && fechaFin.Value.Date >= r.FechaInicio.Date)

                    ||

                    // 3) Nueva indefinida vs existente indefinida (mismo día de inicio)
                    (indefinido && r.Indefinido &&
                     r.FechaInicio.Date == fechaInicio.Date)

                    ||

                    // 4) Nueva indefinida vs existente definida (si la definida cubre ese día)
                    (indefinido && !r.Indefinido &&
                     r.FechaInicio.Date <= fechaInicio.Date && r.FechaFin.HasValue && r.FechaFin.Value.Date >= fechaInicio.Date)
                );

            if (reservaOcupada)
            {
                TempData["Mensaje"] = "El rango seleccionado se superpone con otra reserva.";
                return RedirectToAction(nameof(Index), new { id = equipoId, ubi, lat, lon });
            }


            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.UsuarioId == usuarioId)
                .Select(u => (u.Nombre + " " + u.ApellidoPaterno).Trim())
                .FirstOrDefaultAsync();

            var reserva = new Reserva
            {
                EquipoId = equipoId,
                FechaInicio = fechaInicio,
                FechaFin = indefinido ? null : fechaFin,
                Indefinido = indefinido,
                Estado = "Pendiente",
                UsuarioId = usuarioId,
                Descripcion = descripcion,
                Fecha = DateTime.Now,
                Ubicación = ubicacion
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Reserva creada exitosamente.";
            return RedirectToAction(nameof(Index), new { id = equipoId, ubi, lat, lon });
        }
    }
}
