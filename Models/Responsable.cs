using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models;

public partial class Responsable
{
    [Required]
    public int ResponsableId { get; set; }

    [Required(ErrorMessage = "El nombre del responsable es requerido.")]
    public string Nombre { get; set; }

    public string SegundoNombre { get; set; }

    [Required(ErrorMessage = "El apellido paterno es requerido.")]
    public string ApellidoPaterno { get; set; }

    public string ApellidoMaterno { get; set; }

    public virtual ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}
