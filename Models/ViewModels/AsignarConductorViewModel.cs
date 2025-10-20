using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models.ViewModels
{
    public class AsignarConductorViewModel
    {
        public int ReservaId { get; set; }
        [Required(ErrorMessage= "Seleccione un conductor.")]
        public int ConductorId { get; set; }
    }
}
