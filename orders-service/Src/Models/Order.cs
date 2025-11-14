namespace orders_service.Src.Models
{
    public class Order
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Fecha de creación del pedido.
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Artículos del pedido.
        /// </summary>
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        /// <summary>
        /// Estado del pedido.
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Monto total del pedido.
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// Número de seguimiento del pedido.
        /// </summary>
        public string? TrackingNumber { get; set; }
        /// <summary>
        /// Fecha de entrega estimada del pedido.
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// Razón de cancelamiento del pedido.
        /// </summary>
        public string? CancellationReason { get; set; }
    }
}