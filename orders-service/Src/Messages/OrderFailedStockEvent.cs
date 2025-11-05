namespace Shared.OrderFailedStockEvent
{
    public class OrderFailedStockEvent
    {
        public string OrderId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; }
    }
}