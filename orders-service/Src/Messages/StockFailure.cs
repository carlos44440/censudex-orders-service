namespace orders_service.Src.Messages
{
    public class StockFailure
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int RequestedQuantity { get; set; }
        public int AvailableStock { get; set; }
    }
}