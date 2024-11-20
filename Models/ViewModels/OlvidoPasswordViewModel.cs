using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class OlvidoPasswordViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; }
    }
}