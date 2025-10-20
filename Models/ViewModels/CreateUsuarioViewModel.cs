using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models.ViewModels
{
    public class CreateUsuarioViewModel
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        public string Nombre { get; set; }

        [Display(Name = "Segundo nombre")]
        public string SegundoNombre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        [Display(Name = "Apellido paterno")]
        public string ApellidoPaterno { get; set; }

        [Display(Name = "Apellido materno")]
        public string ApellidoMaterno { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }

        public bool Activo { get; set; }

        public int RolId { get; set; }

        public int? AreaId { get; set; }

    }
}
