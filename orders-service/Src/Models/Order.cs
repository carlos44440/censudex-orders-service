namespace orders_service.Src.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid UserId { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? CancellationReason { get; set; }
    }
}