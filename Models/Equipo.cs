using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class Equipo
{
    public int EquipoId { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public int ResponsableId { get; set; }

    public int TipoEquipoId { get; set; }

    public virtual ICollection<AreaDisponible> AreaDisponibles { get; set; } = new List<AreaDisponible>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Responsable Responsable { get; set; }

    public virtual TipoEquipo TipoEquipo { get; set; }
}
