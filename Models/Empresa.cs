using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class Empresa
{
    public int EmpresaId { get; set; }

    public string Nombre { get; set; }

    public string Descripción { get; set; }

    public virtual ICollection<Area> Areas { get; set; } = new List<Area>();
}
