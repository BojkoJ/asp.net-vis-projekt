using System;
using System.Collections.Generic;

namespace Projekt.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        // Seznam položek objednávky (OrderItems)
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
