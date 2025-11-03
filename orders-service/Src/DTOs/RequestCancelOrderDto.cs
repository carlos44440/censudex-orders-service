using System.ComponentModel.DataAnnotations;

namespace orders_service.Src.DTOs
{
    public class RequestCancelOrderDto
    {
        [Required]
        public string OrderId { get; set; } = string.Empty;

        public string? CancellationReason { get; set; }
    }
}