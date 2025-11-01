using System.ComponentModel.DataAnnotations;

namespace orders_service.Src.DTOs
{
    public class CreateOrderItemDto
    {
       [Required]
        public string ProductId { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}