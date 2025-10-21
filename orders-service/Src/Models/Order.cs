using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orders_service.Src.Models
{
    public class Order
    {
        public string Id { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}