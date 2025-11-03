using orders_service.Src.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace orders_service.Src.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridClient _client;
        private readonly EmailAddress _from;
        public EmailService()
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            _client = new SendGridClient(apiKey);
            _from = new EmailAddress("carlos.arauco@alumnos.ucn.cl", "Carlos");
        }
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