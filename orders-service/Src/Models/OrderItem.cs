namespace orders_service.Src.Models
{
    public class OrderItem
    {
        /// <summary>
        /// Identificador único del artículo.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Identificador único del producto. 
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>
        /// Cantidad requerida del artículo.
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Precio unitario del artículo.
        /// </summary>
        public int UnitPrice { get; set; }
        /// <summary>
        /// Total parcial del pedido.
        /// </summary>
        public int SubTotal { get; set; }
        /// <summary>
        /// Identificador del pedido a la que pertenece este artículo.
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// Pedido al que pertenece este artículo.
        /// </summary>
        public Order? Order { get; set; }
    }
}