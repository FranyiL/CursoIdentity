namespace ProyectoIdentity.Dtos
{
    public class SendEmailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
    }
}
