using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class Responsable
{
    public int ResponsableId { get; set; }

    public string Nombre { get; set; }

    public string SegundoNombre { get; set; }

    public string ApellidoPaterno { get; set; }

    public string ApellidoMaterno { get; set; }

    public virtual ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
}
