using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models;

public partial class Reserva
{
    public int ReservaId { get; set; }
    [Display(Name = "Equipo")]
    public int? EquipoId { get; set; }
    
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; }

    [Display(Name = "Fecha de fin")]
    public DateTime? FechaFin { get; set; }
    public bool Indefinido { get; set; }

    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "Es obligatorio colocar una descripción")]
    public string Descripcion { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string? Ubicación { get; set; }

    public virtual Equipo? Equipo { get; set; }
    public virtual ICollection<DetalleReserva> DetalleReservas { get; set; } = new List<DetalleReserva>();
}
