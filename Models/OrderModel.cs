namespace TGFPIZZAHUB.Models
{
    public class OrderModel
    {
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public string? OrderId { get; set; }
        public string? Formatted { get; set; }
        public string? Json{ get; set; }
        public DateTime? ExpectTime{ get; set; }
    }
}
