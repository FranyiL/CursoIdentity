using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class AccesoViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password, ErrorMessage = "La contraseña no es válida.")]
        public string Password { get; set; }

        [Display(Name = "Recordar datos?")]
        public bool RememberMe { get; set; }
    }
}