namespace orders_service.Src.DTOs
{
    /// <summary>
    /// DTO que representa un ítem dentro de un pedido.
    /// Contiene la información del producto y los valores asociados a la compra.
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Nombre del producto incluido en el pedido.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad solicitada del producto.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        public int UnitPrice { get; set; }

        /// <summary>
        /// Total calculado para este ítem (Quantity * UnitPrice).
        /// </summary>
        public int SubTotal { get; set; }
    }
}
