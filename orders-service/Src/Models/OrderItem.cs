using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orders_service.Src.Models
{
    public class OrderItem
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}