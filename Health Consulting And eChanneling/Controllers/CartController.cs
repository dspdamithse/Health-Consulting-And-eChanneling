using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty!!.";
                return View();
            }
            decimal total = 0m;
            foreach (var item in cart)
            {
                total += item.Total;
            }
            ViewBag.GrandTotal = total;

            return View(cart);
        }
        public ActionResult CartPartial()
        {
            CartViewModel model = new CartViewModel();
            int qty = 0;
            decimal price = 0m;

            //Check Session
            if (Session["cart"]!=null)
            {
                var list = (List<CartViewModel>)Session["cart"];
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }
                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                model.Quantity = 0;
                model.Price = 0;
            }

            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            CartViewModel model = new CartViewModel();

            using (Db db=new Db())
            {
                ProductDTO product = db.Products.Find(id);
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);
                if (productInCart==null)
                {
                    cart.Add(new CartViewModel()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity =1,
                        Price = product.Price,
                        Image =product.ImageName
                    });
                }
                else
                {
                    productInCart.Quantity++;
                }
                int qty = 0;
                decimal price = 0m;
                foreach (var item in cart)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }
                model.Quantity = qty;
                model.Price = price;

                Session["cart"] = cart;
            }
        return PartialView(model);
        }
        
        public JsonResult IncrementProduct(int productId)
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;
            using (Db db = new Db())
            {
                CartViewModel model = cart.FirstOrDefault(x=>x.ProductId ==productId);

                model.Quantity++;

                var result = new {qty = model.Quantity, price = model.Price };
                return Json(result, JsonRequestBehavior.AllowGet);
            }          
        }
        public ActionResult DecrementProduct(int productId)
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;
            using (Db db = new Db())
            {
                CartViewModel model = cart.FirstOrDefault(x => x.ProductId == productId);
                if (model.Quantity>1) {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }
                var result = new { qty = model.Quantity, price = model.Price };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}