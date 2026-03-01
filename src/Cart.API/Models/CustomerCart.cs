namespace Cart.API.Models
{
    public class CustomerCart
    {
        public string CustomerId { get; set; }

        public List<CartItem> Items { get; set; } = new();
    }
}
