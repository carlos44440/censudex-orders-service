namespace orders_service.Src.DTOs
{
    /// <summary>
    /// DTO que representa un pedido completo,
    /// incluyendo información general, estado y detalles del cliente.
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Fecha y hora en que se generó el pedido.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Identificador único del usuario que realizó el pedido.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Lista de productos asociados al pedido.
        /// </summary>
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

        /// <summary>
        /// Estado actual del pedido (pendiente, enviado, cancelado, etc.).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Monto total del pedido, considerando la suma de todos los ítems.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Número de seguimiento asignado para el envío del pedido.
        /// </summary>
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Fecha estimada de entrega del pedido, si aplica.
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Razón por la cual el pedido fue cancelado, si corresponde.
        /// </summary>
        public string? CancellationReason { get; set; }
    }
}
