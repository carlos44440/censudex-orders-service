using System.ComponentModel.DataAnnotations;

namespace orders_service.Src.DTOs
{
    public class RequestCancelOrderDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string UserRole { get; set; } = string.Empty;

        [Required]
        public string OrderId { get; set; } = string.Empty;

        public string? CancellationReason { get; set; }
    }
}