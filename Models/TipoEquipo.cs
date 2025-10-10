using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class TipoEquipo
{
    public int TipoEquipoId { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public virtual ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}
