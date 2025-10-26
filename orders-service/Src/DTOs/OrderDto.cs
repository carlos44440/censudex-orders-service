using System.ComponentModel.DataAnnotations;

namespace orders_service.Src.DTOs
{
    public class OrderDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? CancellationReason { get; set; }
    }
}