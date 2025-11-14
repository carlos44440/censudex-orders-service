using orders_service.Src.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace orders_service.Src.Services
{
    /// <summary>
    /// Servicio encargado del envío de correos electrónicos mediante SendGrid.
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Cliente SendGrid utilizado para enviar correos.
        /// </summary>
        private readonly SendGridClient _client;
        /// <summary>
        /// Dirección de correo del remitente.
        /// </summary>
        private readonly EmailAddress _from;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de correo electrónico,
        /// configurando las credenciales y datos del remitente a partir de variables de entorno.
        /// </summary>
        /// <exception cref="Exception">
        /// Se lanza cuando:
        /// <list type="bullet">
        /// <item>No se encuentra la API Key de SendGrid.</item>
        /// <item>No se encuentra el correo o nombre del remitente.</item>
        /// </list>
        /// </exception>
        public EmailService()
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            _client = new SendGridClient(apiKey);
            var senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL");
            var senderName = Environment.GetEnvironmentVariable("SENDER_NAME");
            _from = new EmailAddress(senderEmail, senderName);
        }

        /// <summary>
        /// Envía un correo electrónico mediante SendGrid.
        /// </summary>
        /// <param name="subject">Asunto del correo.</param>
        /// <param name="toEmail">Correo electrónico del destinatario.</param>
        /// <param name="message">Contenido del mensaje a enviar.</param>
        /// <returns>
        /// Retorna <c>true</c> si el correo fue enviado exitosamente,
        /// <c>false</c> en caso contrario.
        /// </returns>
        public async Task<bool> SendEmailAsync(string subject, string toEmail, string message)
        {
            var to = new EmailAddress(toEmail);
            var htmlContent = $"<pre>{message}</pre>";
            var msg = MailHelper.CreateSingleEmail(_from, to, subject, message, htmlContent);
            var response = await _client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
    }
}