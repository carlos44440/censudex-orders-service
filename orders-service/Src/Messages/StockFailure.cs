namespace orders_service.Src.Messages
{
    /// <summary>
    /// Artículo con fallo de stock.
    /// </summary>
    public class StockFailure
    {
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
        public int RequestedQuantity { get; set; }
        /// <summary>
        /// Stock disponible del producto.
        /// </summary>
        public int AvailableStock { get; set; }
    }
}