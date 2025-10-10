using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class AreaDisponible
{
    public int AreaDisponibleId { get; set; }

    public int AreaId { get; set; }

    public int EquipoId { get; set; }

    public virtual Area Area { get; set; }

    public virtual Equipo Equipo { get; set; }
}
