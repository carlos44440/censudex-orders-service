namespace orders_service.Src.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? CancellationReason { get; set; }
    }
}