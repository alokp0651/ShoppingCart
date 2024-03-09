using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModels;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _context;
        public CartController(DataContext _context)
        {
            this._context = _context;
        }
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.Getjson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartViewModel cartVM = new()
            {
                CartItems = cart,
                Grandtotal = cart.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }
        public async Task<IActionResult> Add(long id)
        {
            Product product = await _context.Products.FindAsync(id);
            List<CartItem> cart = HttpContext.Session.Getjson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();
            if (cartItem == null)
            {
                cart.Add(new CartItem(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);

            TempData["Success"] = "The product has been added";

            return Redirect(Request.Headers["Referer"].ToString());

        }
        public async Task<IActionResult> Decrease(long id)
        {
            List<CartItem> cart = HttpContext.Session.Getjson<List<CartItem>>("Cart");
            CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["Success"] = "The product has been removed";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(long id)
        {
            List<CartItem> cart = HttpContext.Session.Getjson<List<CartItem>>("Cart");
            cart.RemoveAll(p => p.ProductId == id);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["Success"] = "The product has been removed";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear(long id)
        {

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Index");
        }


    }
}
