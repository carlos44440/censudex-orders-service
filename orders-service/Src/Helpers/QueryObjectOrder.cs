using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orders_service.Src.Helpers
{
    public class QueryObjectOrder
    {
        public string? OrderId { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? InitialOrderDate { get; set; }
        public DateTime? FinalOrderDate { get; set; }
    }
}