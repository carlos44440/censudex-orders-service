namespace orders_service.Src.Helpers
{
    public class QueryObjectOrder
    {
        public Guid? OrderId { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime? InitialOrderDate { get; set; }
        public DateTime? FinalOrderDate { get; set; }
    }
}