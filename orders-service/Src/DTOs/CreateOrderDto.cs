namespace orders_service.Src.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerId { get; set; } = string.Empty;
        public List<OrderItemDto> ItemDtos { get; set; } = new List<OrderItemDto>();
    }
}