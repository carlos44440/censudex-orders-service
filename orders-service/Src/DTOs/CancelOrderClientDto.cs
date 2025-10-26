using System.ComponentModel.DataAnnotations;

namespace orders_service.Src.DTOs
{
    public class CancelOrderClientDto
    {
        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        public string OrderId { get; set; } = string.Empty;

        public string? CancellationReason { get; set; }
    }
}