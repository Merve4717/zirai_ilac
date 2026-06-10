using System;
using System.Collections.Generic;

namespace ZiraiIlacERPAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal? TotalAmount { get; set; }

        // Navigation properties
        public Customer? Customer { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
