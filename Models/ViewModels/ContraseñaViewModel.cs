using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models.ViewModels
{
    public class ContraseñaViewModel
    {
        [Required(ErrorMessage = "La contraseña es requerida.")]
        [Display(Name = "Nueva contraseña")]
        [MinLength(5, ErrorMessage = "Debe tener al menos 5 caracteres.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Debes confirmar la contraseña.")]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }
    }
}
