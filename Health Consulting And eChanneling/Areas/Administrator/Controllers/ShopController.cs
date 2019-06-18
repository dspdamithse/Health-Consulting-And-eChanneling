using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    public class ShopController : Controller
    {
        // GET: Administrator/Shop
        public ActionResult Categories()
        {
            List<CategoryViewModel> categoryViewModelList;

            using (Db db = new Db())
            {
                categoryViewModelList = db.Categories
                                .ToArray()
                                .OrderBy(x => x.Sorting)
                                .Select(x => new CategoryViewModel(x))
                                .ToList();
            }
            return View(categoryViewModelList);
        }
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                CategoryDTO dto = new CategoryDTO();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();
            }

            return id;
        }

        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                CategoryDTO dto;
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }

        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);

                db.Categories.Remove(dto);

                db.SaveChanges();
            }
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                CategoryDTO dto = db.Categories.Find(id);

                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                db.SaveChanges();
            }
            return "Ok";
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductViewModel model = new ProductViewModel();

            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Product Name Alredy exist");
                    return View(model);
                }
            }

            int id;

            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();
                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }
            TempData["SM"] = "Successfully added a new product";

            //Image Upload

            //Create nessory file directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            string pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            string pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            string pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            string pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);
            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Image was Not Uploaded- Image format is wrong");
                        return View(model);
                    }
                }

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            return RedirectToAction("Products");
        }

        public ActionResult Products(int? page, int? catId)
        {
            List<ProductViewModel> listOfProductVM;

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                listOfProductVM = db.Products
                                    .ToArray()
                                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                                    .Select(x => new ProductViewModel(x))
                                    .ToList();
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                ViewBag.SelectedCat = catId.ToString();
            }
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);

            ViewBag.OnePageOfProducts = onePageOfProducts;
            return View(listOfProductVM);

        }

        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            ProductViewModel model;
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                if (dto == null)
                {
                    return Content("Product is not available");
                }
                model = new ProductViewModel(dto);

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                               .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult EditProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            int id = model.Id;
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Thumbs")).Select(fn => Path.GetFileName(fn));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Product Name Alredy exist");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }
            TempData["SM"] = "Successfully Updated the product";

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "Image was Not Uploaded- Image format is wrong");
                        return View(model);
                    }
                }
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo dir1 = new DirectoryInfo(pathString1);
                DirectoryInfo dir2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in dir1.GetFiles())
                    file2.Delete();
                foreach (FileInfo file3 in dir2.GetFiles())
                    file3.Delete();

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            return RedirectToAction("EditProduct");
        }
        public ActionResult DeleteProduct(int id)
        {
            //delete from database
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();
            }
            //delete project folders
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            return RedirectToAction("Products");
        }

        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                if (file != null && file.ContentLength > 0)
                {
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);
                }
            }
        }

        public void DeleteImage(int id, string imageName)
        {
            string fullpath1 = Request.MapPath("~/Images/Uploads/Products"+id.ToString()+"/Gallery/"+imageName);
            string fullpath2 = Request.MapPath("~/Images/Uploads/Products"+id.ToString()+"/Gallery/Thumbs"+imageName);

            if (System.IO.File.Exists(fullpath1))
                System.IO.File.Delete(fullpath1);

            if (System.IO.File.Exists(fullpath2))
                System.IO.File.Delete(fullpath2);

        }

    }
}