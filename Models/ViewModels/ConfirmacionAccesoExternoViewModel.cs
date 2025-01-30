using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class ConfirmacionAccesoExternoViewModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Nombre de usuario")]
        public string Name { get; set; }
    }
}
