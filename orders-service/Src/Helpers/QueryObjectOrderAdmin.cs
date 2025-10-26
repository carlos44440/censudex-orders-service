namespace orders_service.Src.Helpers
{
    public class QueryObjectOrderAdmin
    {
        public string? OrderId { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? InitialOrderDate { get; set; }
        public DateTime? FinalOrderDate { get; set; }
    }
}