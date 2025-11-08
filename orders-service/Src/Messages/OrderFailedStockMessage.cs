using orders_service.Src.Messages;

namespace Shared.OrderFailedStockMessage
{
    public class OrderFailedStockMessage
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public List<StockFailure> FailedProducts { get; set; } = new();
        public DateTime FailedAt { get; set; }
    }
}