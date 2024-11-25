using Microsoft.AspNetCore.Identity.UI.Services;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace ProyectoIdentity.Servicios
{

    public class MailJetEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public OpcionesMailJet _opcionesMailJet;


        public MailJetEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _opcionesMailJet = _configuration.GetSection("MailJet").Get<OpcionesMailJet>();
            MailjetClient client = new MailjetClient(_opcionesMailJet.ApiKey,_opcionesMailJet.SecretKey)
            {
                
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
               .Property(Send.Messages, new JArray {
                new JObject {
                 {"From", new JObject {
                  {"Email", "franyiliriano@protonmail.com"},
                  {"Name", "Liriano Software Solutions"}
                  }},
                 {"To", new JArray {
                  new JObject {
                   {"Email", email},
                   {"Name", email}
                   }
                  }},
                 {"Subject", subject},
                 {"HTMLPart", htmlMessage}
                 }
                   });
            await client.PostAsync(request);
            /* if (response.IsSuccessStatusCode)
             {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
             }
             else
             {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
             }*/
        }

    }
}
