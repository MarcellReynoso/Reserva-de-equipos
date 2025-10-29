using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models;

public partial class Equipo
{
    public int EquipoId { get; set; }

    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    public string Nombre { get; set; }

    public string? Descripcion { get; set; }

    public int ResponsableId { get; set; }

    public int TipoEquipoId { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string? ImagenUrl { get; set; }

    public string? ImagenJson { get; set; }

    public virtual ICollection<AreaDisponible> AreaDisponibles { get; set; } = new List<AreaDisponible>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Responsable Responsable { get; set; }

    public virtual TipoEquipo TipoEquipo { get; set; }
}
