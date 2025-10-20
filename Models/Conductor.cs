using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models
{
    public partial class Conductor
    {
        [Key]
        public int ConductorId { get; set; }

        [Required(ErrorMessage="El nombre es obligatorio.")]
        [MaxLength(200)]
        public string Nombre { get; set; }

        [MaxLength(200)]
        public string SegundoNombre { get; set; }

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [MaxLength(200)]
        public string ApellidoPaterno { get; set; }

        [MaxLength(200)]
        public string ApellidoMaterno { get; set; }

        public bool Disponible { get; set; }

        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
