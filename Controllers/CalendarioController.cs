using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Reserva_de_equipos.Models;
using System.Threading.Tasks;

namespace Reserva_de_equipos.Controllers
{
    [Authorize]
    public class CalendarioController : Controller
    {
        private readonly DbReservaContext _context;
        private readonly IConfiguration _configuration;

        public CalendarioController(DbReservaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(int? tipoEquipoId)
        {
            var items = await _context.Reservas
                .AsNoTracking()
                .Include(r => r.usuario)
                .Select(r => new
                {
                    id = r.ReservaId,
                    start = r.FechaInicio,
                    end = r.Indefinido ? (DateTime?)null : r.FechaFin,
                    ubicacion = r.Ubicación,
                    title = r.usuario.Nombre + " " + r.usuario.ApellidoPaterno,
                    estado = r.Estado,
                    descripcion = r.Descripcion
                })
                .ToListAsync();

            ViewBag.Reservas = JsonConvert.SerializeObject(items);

            ViewBag.MapboxToken = _configuration["Mapbox:Token"];

            var tipos = await _context.TipoEquipos
                .Where(t => _context.Equipos.Any(e => e.TipoEquipoId == t.TipoEquipoId && e.Disponible))
                .Select(t => new SelectListItem { Value = t.TipoEquipoId.ToString(), Text = t.Nombre })
                .OrderBy(t => t.Text)
                .ToListAsync();
            ViewBag.TiposEquipo = new SelectList(tipos, "Value", "Text");


            var equipos = await _context.Equipos
                .Where(e => e.Disponible)
                .Select(e => new SelectListItem { Value = e.EquipoId.ToString(), Text = e.Nombre })
                .ToListAsync();
            ViewBag.Equipos = new SelectList(equipos, "Value", "Text");

            return View();
        }
    }
}
