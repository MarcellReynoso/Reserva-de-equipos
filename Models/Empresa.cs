using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models;

public partial class Empresa
{
    [Required]
    public int EmpresaId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }

    public string Descripción { get; set; }

    public virtual ICollection<Area> Areas { get; set; } = new List<Area>();
}
