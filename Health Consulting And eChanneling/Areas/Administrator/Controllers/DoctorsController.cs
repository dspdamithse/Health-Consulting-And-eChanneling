using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Doctors;
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
        [ActionName("new-doctor-registration")]
        public ActionResult AddNewDoctor()
        {
            DoctorViewModel model = new DoctorViewModel();

            using (Db db = new Db())
            {
                model.SpecialistArea = new SelectList(db.SpecialistArea.ToList(), "Id", "Name");
            }
            return View("AddNewDoctor", model);
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
                doctor.SLMC_Reg_No = model.SLMC_Reg_No;
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

            return RedirectToAction("view-all-registed-doctors-list");
        }

        [ActionName("view-all-registed-doctors-list")]
        public ActionResult Doctors(int? page, int? catId)
        {
            List<DoctorViewModel> listOfDoctorVM;

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                listOfDoctorVM = db.Doctors
                                    .ToArray()
                                    .Where(x => catId == null || catId == 0 || x.SpecialistAreaId == catId)
                                    .Select(x => new DoctorViewModel(x))
                                    .ToList();
                ViewBag.SpecialistArea = new SelectList(db.SpecialistArea.ToList(), "Id", "Name");
                ViewBag.SelectedCat = catId.ToString();
            }
            var onePageOfProducts = listOfDoctorVM.ToPagedList(pageNumber, 10);

            ViewBag.OnePageOfProducts = onePageOfProducts;
            return View("Doctors", listOfDoctorVM);

        }

        [HttpGet]
        public ActionResult EditDoctor(int id)
        {
            DoctorViewModel model;
            using (Db db = new Db())
            {
                DoctorDTO dto = db.Doctors.Find(id);
                if (dto == null)
                {
                    return Content("Doctor is Alredy Registered");
                }
                model = new DoctorViewModel(dto);

                model.SpecialistArea = new SelectList(db.SpecialistArea.ToList(), "Id", "Name");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult EditDoctor(DoctorViewModel model, HttpPostedFileBase file)
        {
            int id = model.Id;
            using (Db db = new Db())
            {
                model.SpecialistArea = new SelectList(db.SpecialistArea.ToList(), "Id", "Name");
            }           

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                if (db.Doctors.Where(x => x.Id != id).Any(x => x.Username == model.Username))
                {
                    ModelState.AddModelError("", "Username Alredy exist");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                DoctorDTO doctor = new DoctorDTO();
                doctor.FirstName = model.FirstName;
                doctor.LastName = model.LastName;
                doctor.Username = model.Username;
                doctor.SLMC_Reg_No = model.SLMC_Reg_No;
                doctor.ContactNumber = model.ContactNumber;
                doctor.Image = model.Image;
                doctor.About = model.About;
                doctor.SpecialistAreaId = model.SpecialistAreaId;

                SpecialistAreaDTO spAreaDTO = db.SpecialistArea.FirstOrDefault(x => x.Id == model.SpecialistAreaId);
                doctor.SpecialistAreaName = spAreaDTO.Name;

                db.SaveChanges();
            }
            TempData["SM"] = "Successfully Updated the Doctors Info";

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

                string pathString1 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString());
                string pathString2 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo dir1 = new DirectoryInfo(pathString1);
                DirectoryInfo dir2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in dir1.GetFiles())
                    file2.Delete();
                foreach (FileInfo file3 in dir2.GetFiles())
                    file3.Delete();

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    DoctorDTO dto = db.Doctors.Find(id);
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
            return RedirectToAction("view-all-registed-doctors-list");
        }

        [ActionName("view-doctors-view-details")]
        public ActionResult DoctorDetails(int id)
        {
            DoctorViewModel model;

            using (Db db = new Db())
            {
                DoctorDTO dto = db.Doctors.Find(id);

                if (dto == null)
                {
                    return Content("Page is not Exist");
                }

                model = new DoctorViewModel(dto);

            }
            return View("DoctorDetails", model);
        }

        public ActionResult DeleteDoctor(int id)
        {
            //delete from database
            using (Db db = new Db())
            {
                DoctorDTO dto = db.Doctors.Find(id);
                db.Doctors.Remove(dto);
                db.SaveChanges();
            }
            //delete project folders
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            return RedirectToAction("view-all-registed-doctors-list");
        }

    }
}