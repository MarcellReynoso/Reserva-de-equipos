using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
