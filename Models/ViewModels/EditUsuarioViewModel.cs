using System.ComponentModel.DataAnnotations;

namespace Reserva_de_equipos.Models.ViewModels
{
    public class EditUsuarioViewModel
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        public string Nombre { get; set; }

        public string SegundoNombre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido.")]
        public string Correo { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Campo obligatorio.")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }

        public bool Activo { get; set; }

        [Required]
        public int RolId { get; set; }

        public int? AreaId { get; set; }
    }
}
