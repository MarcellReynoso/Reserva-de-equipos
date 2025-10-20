using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models;

public partial class AreaDisponible
{
    public int AreaDisponibleId { get; set; }

    [Required(ErrorMessage = "Area obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un área válida.")]
    public int? AreaId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un equipo válido.")]
    public int? EquipoId { get; set; }

    public virtual Area Area { get; set; }

    public virtual Equipo Equipo { get; set; }
}
