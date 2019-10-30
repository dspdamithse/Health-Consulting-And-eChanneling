using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.News;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        public ActionResult NewsList()
        {
            List<NewsViewModel> newsList;

            using (Db db = new Db())
            {
                newsList = db.News.ToArray().OrderBy(x => x.Id).Select(x => new NewsViewModel(x)).ToList();
            }

            //Return view with list
            return View(newsList);
        }
        [HttpGet]
        public ActionResult AddNewNewsArticle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddNewNewsArticle(NewsViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            int userid;
            using (Db db = new Db())
            {
                string slug;
                if (db.News.Any(x => x.Title.Equals(model.Title)))
                {
                    ModelState.AddModelError("", "Title Alredy exist");
                    model.Title = "";
                    return View("AddNewNewsArticle", model);
                }
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                NewsDTO newsDto = new NewsDTO()
                {
                    Title = model.Title,
                    Slug = slug,
                    Content = model.Content,
                };

                db.News.Add(newsDto);

                db.SaveChanges();

                int id = newsDto.Id;
                userid = newsDto.Id;

            }
            TempData["SuccessMsg"] = "New News Articles has sucessfully added";

            var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\News", Server.MapPath(@"\")));

            string pathString1 = Path.Combine(originalDirectory.ToString(), "Images");
            string pathString2 = Path.Combine(originalDirectory.ToString(), "Images\\" + userid.ToString());
            string pathString3 = Path.Combine(originalDirectory.ToString(), "Images\\" + userid.ToString() + "\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "Image was Not Uploaded- Image format is wrong");
                        return View("CreateAccont", model);
                    }
                }

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    NewsDTO dto = db.News.Find(userid);
                    dto.Image = imageName;

                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            return RedirectToAction("NewsList");
        }

        [HttpGet]
        public ActionResult EditNews(int id)
        {
            NewsViewModel model;
            using (Db db = new Db())
            {
                NewsDTO dto = db.News.Find(id);
                if (dto == null)
                {
                    return Content("The Page does not exist");
                }

                model = new NewsViewModel(dto);

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditNews(NewsViewModel model, HttpPostedFileBase file)
        {
            int id;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                id = model.Id;
                string slug;
                NewsDTO dto = db.News.Find(id);

                dto.Title = model.Title;

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.News.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    db.News.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Title or Slug Alredy Exist");
                    return View(model);
                }
                dto.Slug = slug;
                dto.Content = model.Content;
                

                db.SaveChanges();
            }
            TempData["SM"] = "New Article has successfully Updated";

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
                var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\News", Server.MapPath(@"\")));

                string pathString1 = Path.Combine(originalDirectory.ToString(), "Images\\" + id.ToString());
                string pathString2 = Path.Combine(originalDirectory.ToString(), "Images\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo dir1 = new DirectoryInfo(pathString1);
                DirectoryInfo dir2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in dir1.GetFiles())
                    file2.Delete();
                foreach (FileInfo file3 in dir2.GetFiles())
                    file3.Delete();

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    NewsDTO dto = db.News.Find(id);
                    dto.Image = imageName;

                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            return RedirectToAction("NewsList");
        }

        public ActionResult DetailsNews(int id)
        {
            NewsViewModel model;

            using (Db db = new Db())
            {
                NewsDTO dto = db.News.Find(id);

                if (dto == null)
                {
                    return Content("Lesson is not Exist");
                }

                model = new NewsViewModel(dto);

            }
            return View(model);
        }
    }
}