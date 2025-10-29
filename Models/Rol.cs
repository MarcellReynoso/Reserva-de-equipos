using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class Rol
{
    public int RolId { get; set; }

    public string Nombre { get; set; }

    public string Descripción { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<Responsable> Responsables { get; set; } = new List<Responsable>();
}
