using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProyectoIdentity.Models;
using System.Net;
using System.Net.Mail;

namespace ProyectoIdentity.Servicios
{
    public class Message : IMessage
    {
        public GmailSettings _gmailSettings { get; }

        public Message(IOptions<GmailSettings> gmailSettings)
        {
            _gmailSettings = gmailSettings.Value;
        }
        public Task<IActionResult> SendEmail(string subject, string body, string to)
        {
            try
            {
                var fromEmail = _gmailSettings.Username;
                var password = _gmailSettings.Password;
                
                var message = new MailMessage();
                message.From = new MailAddress(fromEmail);
                message.Subject = subject;
                message.To.Add(new MailAddress(to));
                message.Body = body;
                //Permite que en el body se pueda insertar código html
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = _gmailSettings.Port,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true
                };

                smtpClient.Send(message);
                return Task.FromResult<IActionResult>(new OkResult());
            }
            catch (Exception ex)
            {

                throw new Exception("No se pudo enviar el email", ex);
            }
        }
    }
}
