using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Doctors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    public class DoctorsController : Controller
    {
        public ActionResult SpecialistCategories()
        {
            List<SpecialistAreaViewModel> specialistAreaViewModelList;


            using (Db db = new Db())
            {
                specialistAreaViewModelList = db.SpecialistArea
                                .ToArray()
                                .OrderBy(x => x.Sorting)
                                .Select(x => new SpecialistAreaViewModel(x))
                                .ToList();
            }
            return View(specialistAreaViewModelList);
        }
        [HttpPost]
        public string AddNewSpecialistArea(string catName)
        {
            string id;
            using (Db db = new Db())
            {
                if (db.SpecialistArea.Any(x => x.Name == catName))
                    return "titletaken";

                SpecialistAreaDTO dto = new SpecialistAreaDTO();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.SpecialistArea.Add(dto);
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
                SpecialistAreaDTO dto;
                foreach (var catId in id)
                {
                    dto = db.SpecialistArea.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }
        public ActionResult DeleteSpecialistAreaCategory(int id)
        {
            using (Db db = new Db())
            {
                SpecialistAreaDTO dto = db.SpecialistArea.Find(id);

                db.SpecialistArea.Remove(dto);

                db.SaveChanges();
            }
            return RedirectToAction("SpecialistCategories");
        }

        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.SpecialistArea.Any(x => x.Name == newCatName))
                    return "titletaken";

                SpecialistAreaDTO dto = db.SpecialistArea.Find(id);

                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                db.SaveChanges();
            }
            return "Ok";
        }

        [HttpGet]
        public ActionResult AddNewDoctor()
        {
            DoctorViewModel model = new DoctorViewModel();

            using (Db db = new Db())
            {
                model.SpecialistArea = new SelectList(db.SpecialistArea.ToList(), "Id", "Name");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddNewDoctor(DoctorViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.SpecialistArea = new SelectList(db.SpecialistArea.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                if (db.Doctors.Any(x => x.Username == model.Username))
                {
                    model.SpecialistArea = new SelectList(db.SpecialistArea.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Username Alredy exist");
                    return View(model);
                }
            }

            int id;

            using (Db db = new Db())
            {
                DoctorDTO doctor = new DoctorDTO();
                doctor.FirstName = model.FirstName;
                doctor.LastName = model.LastName;
                doctor.Username = model.Username;
                doctor.RegNumber = model.RegNumber;
                doctor.ContactNumber = model.ContactNumber;
                doctor.Image = model.Image;
                doctor.About = model.About;
                doctor.SpecialistAreaId = model.SpecialistAreaId;
                doctor.SpecialistAreaName = model.SpecialistAreaName;

                SpecialistAreaDTO specDTO = db.SpecialistArea.FirstOrDefault(x => x.Id == model.SpecialistAreaId);
                doctor.SpecialistAreaName = specDTO.Name;

                db.Doctors.Add(doctor);
                db.SaveChanges();

                id = doctor.Id;
            }
            TempData["SM"] = "New Doctor Has Registered Successfully";

            //Image Upload

            //Create nessory file directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            string pathString1 = Path.Combine(originalDirectory.ToString(), "Doctors");
            string pathString2 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString());
            string pathString3 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString() + "\\Thumbs");
            string pathString4 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString() + "\\Gallery");
            string pathString5 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString() + "\\Gallery\\Thumbs");

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
                        model.SpecialistArea = new SelectList(db.SpecialistArea.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Image was Not Uploaded- Image format is wrong");
                        return View(model);
                    }
                }

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    DoctorDTO dto = db.Doctors.Find(id);
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

            return RedirectToAction("AddNewDoctor");
        }


    }
}