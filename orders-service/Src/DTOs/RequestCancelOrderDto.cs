using System.ComponentModel.DataAnnotations;
using orders_service.Src.Messages;

namespace orders_service.Src.DTOs
{
    public class RequestCancelOrderDto
    {
        [Required]
        public Guid OrderId { get; set; }
        public string? CancellationReason { get; set; }
        public List<StockFailure>? FailedProducts { get; set; }
    }
}