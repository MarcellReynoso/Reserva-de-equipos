using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models;

public partial class TipoEquipo
{
    [Required]
    public int TipoEquipoId { get; set; }

    [Required(ErrorMessage = "El nombre el tipo de equipo es obligatorio.")]
    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public virtual ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}
