using Microsoft.AspNetCore.Mvc;

namespace ProyectoIdentity.Servicios
{
    public interface IMessage
    {
        public Task<IActionResult> SendEmail(string subject, string body, string to);
    }
}
