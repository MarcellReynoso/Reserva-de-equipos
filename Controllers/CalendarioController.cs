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

        public CalendarioController(DbReservaContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Obtener los eventos para el calendario
            List<Reserva> reservas = _context.Reservas.ToList();
            List<object> items = new List<object>();
            foreach (var reserva in reservas)
            {
                var item = new
                {
                    id = reserva.ReservaId,
                    start = reserva.FechaInicio,
                    end = reserva.FechaFin,
                    ubicacion = reserva.Ubicación,
                    title = reserva.Descripcion,
                    estado = reserva.Estado
                };
                items.Add(item);
            }
            ViewBag.Reservas = JsonConvert.SerializeObject(items);

            // Obtener la lista de equipos para el formulario Create
            ViewBag.Equipos = await _context.Equipos
                .Select(e => new SelectListItem
                {
                    Value = e.EquipoId.ToString(),
                    Text = e.Nombre
                })
                .ToListAsync();

            return View();
        }
    }
}
