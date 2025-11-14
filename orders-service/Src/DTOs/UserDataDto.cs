namespace orders_service.Src.DTOs
{
    /// <summary>
    /// DTO que representa la información básica del usuario
    /// utilizada en las operaciones relacionadas a pedidos.
    /// </summary>
    public class UserDataDto
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del usuario asociado a la solicitud.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario (Admin o Client).
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico del usuario.
        /// </summary>
        public string EmailAddress { get; set; } = string.Empty;
    }
}
