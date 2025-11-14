using orders_service.Src.Messages;

namespace Shared.OrderFailedStockMessage
{
    /// <summary>
    /// Clase para enviar el mensaje de fallo de stock del pedido.
    /// </summary>
    public class OrderFailedStockMessage
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// Razon del fallo de stock.
        /// </summary>
        public string Reason { get; set; } = string.Empty;
        /// <summary>
        /// Productos con fallos de stock. 
        /// </summary>
        public List<StockFailure> FailedProducts { get; set; } = new();
        /// <summary>
        /// Fecha de fallo de stock.
        /// </summary>
        public DateTime FailedAt { get; set; }
    }
}