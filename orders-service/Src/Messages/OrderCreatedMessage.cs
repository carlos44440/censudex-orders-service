using orders_service.Src.Messages;

namespace Shared.OrderCreatedMessage
{
    /// <summary>
    /// Clase para enviar el mensaje de pedido creado.
    /// </summary>
    public class OrderCreatedMessage
    {
        /// <summary>
        /// Identificador único del pedido. 
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// Identificador único del cliente.
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Artículos del pedido.
        /// </summary>
        /// <returns></returns>
        public List<OrderItemMessage> Items { get; set; } = new();
        /// <summary>
        /// Fecha de creación del pedido.
        /// </summary>
        /// <value></value>
        public DateTime CreatedAt { get; set; }
    }
}