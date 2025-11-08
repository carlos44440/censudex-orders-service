using System.ComponentModel.DataAnnotations;

namespace orders_service.Src.DTOs
{
    public class CreateOrderItemDto
    {
       [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}