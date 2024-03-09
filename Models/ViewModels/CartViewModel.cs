namespace ShoppingCart.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> CartItems{ get; set; }
        public decimal Grandtotal { get; set; }
    }
}
