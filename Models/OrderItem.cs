namespace Projekt.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }

        // Reference na objednávku a variantu produktu
        public Order Order { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
