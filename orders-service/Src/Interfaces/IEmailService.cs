namespace orders_service.Src.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio encargado del envío de correos electrónicos.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico con asunto, destinatario y contenido especificado.
        /// </summary>
        /// <param name="subject">Asunto del correo electrónico.</param>
        /// <param name="toEmail">Dirección de correo del destinatario.</param>
        /// <param name="message">Contenido del mensaje en formato texto o HTML.</param>
        /// <returns>
        /// Retorna <c>true</c> si el correo fue enviado exitosamente; de lo contrario, retorna <c>false</c>.
        /// </returns>
        Task<bool> SendEmailAsync(string subject, string toEmail, string message);
    }
}
