namespace orders_service.Src.DTOs
{
    public class CreateOrderDto
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}