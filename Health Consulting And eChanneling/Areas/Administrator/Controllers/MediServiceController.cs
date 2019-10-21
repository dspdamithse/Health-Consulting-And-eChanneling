using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.MediService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    public class MediServiceController : Controller
    {
        public ActionResult MedicalServiceList()
        {
            List<MediServiceViewModel> mediServiceList;

            using (Db db = new Db())
            {
                mediServiceList = db.MediService.ToArray().OrderByDescending(x => x.Id).Select(x => new MediServiceViewModel(x)).ToList();
            }

            return View(mediServiceList);
        }

        [HttpGet]
        public ActionResult CreateNewService()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateNewService(MediServiceViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            int serviceid;
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
                MediServiceDTO mediDto = new MediServiceDTO()
                {
                    Title = model.Title,
                    Slug = slug,
                    Content = model.Content,
                };

                db.MediService.Add(mediDto);

                db.SaveChanges();

               serviceid = mediDto.Id;


            }
            TempData["SuccessMsg"] = "New News Articles has sucessfully added";

            var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\MediService", Server.MapPath(@"\")));

            string pathString1 = Path.Combine(originalDirectory.ToString(), "Images");
            string pathString2 = Path.Combine(originalDirectory.ToString(), "Images\\" + serviceid.ToString());
            string pathString3 = Path.Combine(originalDirectory.ToString(), "Images\\" + serviceid.ToString() + "\\Thumbs");

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
                    MediServiceDTO dto = db.MediService.Find(serviceid);
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
            return RedirectToAction("MedicalServiceList");
        }

        [HttpGet]
        public ActionResult EditMediService(int id)
        {
            MediServiceViewModel model;
            using (Db db = new Db())
            {
                MediServiceDTO dto = db.MediService.Find(id);
                if (dto == null)
                {
                    return Content("Service is not available");
                }

                model = new MediServiceViewModel(dto);

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditMediService(MediServiceViewModel model, HttpPostedFileBase file)
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
                MediServiceDTO dto = db.MediService.Find(id);

                dto.Title = model.Title;

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.MediService.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    db.MediService.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Title or Slug Alredy Exist");
                    return View(model);
                }
                dto.Slug = slug;
                dto.Content = model.Content;


                db.SaveChanges();
            }
            TempData["SM"] = "New Service has successfully Updated";

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
                var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\MediService", Server.MapPath(@"\")));

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
                    MediServiceDTO dto = db.MediService.Find(id);
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
            return RedirectToAction("MedicalServiceList");
        }

        public ActionResult DeleteMediService(int id)
        {
            using (Db db = new Db())
            {
                MediServiceDTO dto = db.MediService.Find(id);
                db.MediService.Remove(dto);
                db.SaveChanges();
            }

            var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\MediService", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Images\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            return RedirectToAction("MedicalServiceList");
        }

        public ActionResult MediServiceDetails(int id)
        {
            MediServiceViewModel model;

            using (Db db = new Db())
            {
                MediServiceDTO dto = db.MediService.Find(id);

                if (dto == null)
                {
                    return Content("Lesson is not Exist");
                }

                model = new MediServiceViewModel(dto);

            }
            return View(model);
        }
    }
}