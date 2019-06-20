using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }
        public ActionResult CategoryMenuPartial()
        {
            List<CategoryViewModel> categoryViewModelList;

            using(Db db = new Db())
            {
                categoryViewModelList = db.Categories.ToArray().OrderBy(x=>x.Sorting).Select(x=>new CategoryViewModel(x)).ToList();
            }

            return PartialView(categoryViewModelList);
        }
        public ActionResult Category(string name)
        {
            List<ProductViewModel> productViewModelList;
            using (Db db =new Db())
            {
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                productViewModelList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductViewModel(x)).ToList();

                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;

            }
            return View(productViewModelList);
        }
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            ProductViewModel model;
            ProductDTO dto;

            int id = 0;
            using (Db db = new Db())
            {
                if (!db.Products.Any(x=>x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                id = dto.Id;

                model = new ProductViewModel(dto);
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                               .Select(fn => Path.GetFileName(fn));
            return View("ProductDetails", model);
        }

    }
}