using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class DetalleReserva
{
    public int DetalleReservaId { get; set; }

    public string Descripcion { get; set; }

    public int CantidadEquipos { get; set; }

    public int ReservaId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string Ubicación { get; set; }

    public virtual Reserva Reserva { get; set; }
}
