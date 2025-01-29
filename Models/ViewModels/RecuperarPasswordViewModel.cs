using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class RecuperarPasswordViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password, ErrorMessage = "La contraseña no es válida.")]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
        [Compare("Password", ErrorMessage = "La contraseña y confirmación de contraseña no coinciden")]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar contraseña")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
