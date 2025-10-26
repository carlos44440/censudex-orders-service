using System.ComponentModel.DataAnnotations;

namespace orders_service.Src.DTOs
{
    public class OrderItemDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;
        
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "La precio debe ser mayor a 0")]
        public int UnitPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "La subtotal debe ser mayor a 0")]
        public int SubTotal { get; set; }
    }
}