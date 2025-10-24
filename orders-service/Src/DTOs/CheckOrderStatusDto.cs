namespace orders_service.Src.DTOs
{
    public class CheckOrderStatusDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}