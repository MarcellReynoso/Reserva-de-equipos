using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models;

public partial class Area
{
    public int AreaId { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public string Descripción { get; set; }

    [Required]
    public int? EmpresaId { get; set; }

    public virtual ICollection<AreaDisponible> AreaDisponibles { get; set; } = new List<AreaDisponible>();

    public virtual Empresa Empresa { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
