using System;
using System.Collections.Generic;

namespace Reserva_de_equipos.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; }

    public string SegundoNombre { get; set; }

    public string ApellidoPaterno { get; set; }

    public string ApellidoMaterno { get; set; }

    public string Correo { get; set; }

    public string Password { get; set; }

    public string Username { get; set; }

    public bool Activo { get; set; }

    public int RolId { get; set; }

    public int? AreaId { get; set; }

    public virtual Area Area { get; set; }

    public virtual Rol Rol { get; set; }
}
