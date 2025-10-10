using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class Area
{
    public int AreaId { get; set; }

    public string Nombre { get; set; }

    public string Descripción { get; set; }

    public int? EmpresaId { get; set; }

    public virtual ICollection<AreaDisponible> AreaDisponibles { get; set; } = new List<AreaDisponible>();

    public virtual Empresa Empresa { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
