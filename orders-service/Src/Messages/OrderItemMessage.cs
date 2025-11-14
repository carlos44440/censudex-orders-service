namespace orders_service.Src.Messages
{
    /// <summary>
    /// Articulo del pedido
    /// </summary>
    public class OrderItemMessage
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Cantidad del producto.
        /// </summary>
        public int Quantity { get; set; }
    }
}