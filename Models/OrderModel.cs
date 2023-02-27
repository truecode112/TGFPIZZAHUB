namespace TGFPIZZAHUB.Models
{
    public class OrderModel
    {
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public string? OrderId { get; set; }
        public string? Formated { get; set; }
        public string? Json{ get; set; }
        public bool? IsOrdered{ get; set; }
    }
}
