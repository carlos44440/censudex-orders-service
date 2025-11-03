namespace orders_service.Src.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string subject, string toEmail, string message);
    }
}