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
using Health_Consulting_And_eChanneling.Models.ViewModels.Account;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    public class DoctorsController : Controller
    {
        [HttpGet]
        public ActionResult DoctorRegistration()
        {
            DoctorViewModel model = new DoctorViewModel();

            using (Db db = new Db())
            {
                model.SpecialistArea = new SelectList(db.DoctorSpecialist.ToList(), "Id", "Name");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DoctorRegistration(DoctorViewModel model, HttpPostedFileBase file)
        {
            int userid;
            int doctorid;
            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", "Username " + model.Username + " Alredy exist");
                    model.Username = "";
                    return View("DoctorRegistration", model);
                }
                if (db.Users.Any(x => x.EmailAddress.Equals(model.EmailAddress)))
                {
                    ModelState.AddModelError("", "Email " + model.EmailAddress + " Alredy exist");
                    model.EmailAddress = "";
                    return View("DoctorRegistration", model);
                }

            }

            using (Db db = new Db())
            {
                UserDTO userDTO = new UserDTO();
                userDTO.FirstName = model.FirstName;
                userDTO.LastName = model.LastName;
                userDTO.Username = model.Username;
                userDTO.EmailAddress = model.EmailAddress;
                userDTO.Password = "12345";
                userDTO.RoleConfirm = 1;

                db.Users.Add(userDTO);
                db.SaveChanges();

                userid = userDTO.Id;
            }
            using (Db db = new Db())
            {
                DoctorDTO doctorDTO = new DoctorDTO();
                doctorDTO.FirstName = model.FirstName;
                doctorDTO.LastName = model.LastName;
                doctorDTO.Username = model.Username;
                doctorDTO.SLMC_Reg_No = model.SLMC_Reg_No;
                doctorDTO.ContactNumber = model.ContactNumber;
                doctorDTO.EmailAddress = model.EmailAddress;
                doctorDTO.About = model.About;
                doctorDTO.SpecialistAreaId = model.SpecialistAreaId;
                doctorDTO.SpecialistAreaName = model.SpecialistAreaName;
                doctorDTO.UserId = userid;
                doctorDTO.Password = "12345";

                DoctorSpecialistDTO specDTO = db.DoctorSpecialist.FirstOrDefault(x => x.Id == model.SpecialistAreaId);
                doctorDTO.SpecialistAreaName = specDTO.Name;

                db.Doctors.Add(doctorDTO);
                db.SaveChanges();

                int id = userid;
                doctorid = doctorDTO.Id;

                UserRoleDTO userRoleDto = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 3
                };
                db.UserRoles.Add(userRoleDto);
                db.SaveChanges();
            }
            TempData["SM"] = "Successfully Registered";
            var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\Registration", Server.MapPath(@"\")));

            string pathString1 = Path.Combine(originalDirectory.ToString(), "Doctors");
            string pathString2 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + doctorid.ToString());
            string pathString3 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + doctorid.ToString() + "\\Thumbs");

            string pathString4 = Path.Combine(originalDirectory.ToString(), "Users");
            string pathString5 = Path.Combine(originalDirectory.ToString(), "Users\\" + userid.ToString());
            string pathString6 = Path.Combine(originalDirectory.ToString(), "Users\\" + userid.ToString() + "\\Thumbs");

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
            if (!Directory.Exists(pathString6))
                Directory.CreateDirectory(pathString6);

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
                    DoctorDTO dto = db.Doctors.Find(doctorid);
                    dto.Image = imageName;

                    db.SaveChanges();

                    UserDTO udto = db.Users.Find(userid);
                    udto.ProfileImage = imageName;

                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                var path3 = string.Format("{0}\\{1}", pathString5, imageName);
                var path4 = string.Format("{0}\\{1}", pathString6, imageName);

                file.SaveAs(path);
                file.SaveAs(path3);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
                img.Save(path4);
            }

            return Redirect("~/Administrator/Doctors/Doctors");
        }
        public ActionResult Doctors(int? page, int? catId)
        {
            List<DoctorViewModel> listOfDoctorVM;

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                listOfDoctorVM = db.Doctors
                                    .ToArray()
                                    .Where(x => catId == null || catId == 0 || x.SpecialistAreaId == catId)
                                    .OrderByDescending(x=>x.Id)
                                    .Select(x => new DoctorViewModel(x))
                                    .ToList();
                ViewBag.SpecialistArea = new SelectList(db.DoctorSpecialist.ToList(), "Id", "Name");
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
                    return Content("Doctor is not available");
                }
                model = new DoctorViewModel(dto);

                model.SpecialistArea = new SelectList(db.DoctorSpecialist.ToList(), "Id", "Name");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult EditDoctor(DoctorViewModel model, HttpPostedFileBase file)
        {
            int id = model.Id;
            int userid=model.UserId;
            using (Db db = new Db())
            {
                model.SpecialistArea = new SelectList(db.DoctorSpecialist.ToList(), "Id", "Name");
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
                UserDTO udto = db.Users.Find(userid);
                udto.FirstName = model.FirstName;
                udto.LastName = model.LastName;
                udto.EmailAddress = model.EmailAddress;
                udto.Username = model.Username;
                udto.RoleConfirm =1;

                db.SaveChanges();
            }
            using (Db db = new Db())
            {
                DoctorDTO ddto = db.Doctors.Find(id);
                ddto.FirstName = model.FirstName;
                ddto.LastName = model.LastName;
                ddto.EmailAddress = model.EmailAddress;
                ddto.Username = model.Username;
                ddto.SLMC_Reg_No = model.SLMC_Reg_No;
                ddto.ContactNumber = model.ContactNumber;
                ddto.About = model.About;
                ddto.SpecialistAreaId = model.SpecialistAreaId;
                ddto.UserId = userid;

                DoctorSpecialistDTO spAreaDTO = db.DoctorSpecialist.FirstOrDefault(x => x.Id == model.SpecialistAreaId);
                ddto.SpecialistAreaName = spAreaDTO.Name;

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
                var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\Registration", Server.MapPath(@"\")));

                string pathString1 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString());
                string pathString2 = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString() + "\\Thumbs");

                string pathString3 = Path.Combine(originalDirectory.ToString(), "Users\\" + userid.ToString());
                string pathString4 = Path.Combine(originalDirectory.ToString(), "Users\\" + userid.ToString() + "\\Thumbs");

                DirectoryInfo dir1 = new DirectoryInfo(pathString1);
                DirectoryInfo dir2 = new DirectoryInfo(pathString2);

                DirectoryInfo dir3 = new DirectoryInfo(pathString3);
                DirectoryInfo dir4 = new DirectoryInfo(pathString4);

                foreach (FileInfo file1 in dir1.GetFiles())
                    file1.Delete();
                foreach (FileInfo file2 in dir2.GetFiles())
                    file2.Delete();

                foreach (FileInfo file3 in dir3.GetFiles())
                    file3.Delete();
                foreach (FileInfo file4 in dir4.GetFiles())
                    file4.Delete();

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    DoctorDTO dto = db.Doctors.Find(id);
                    dto.Image = imageName;
                    db.SaveChanges();

                    UserDTO udto = db.Users.Find(userid);
                    udto.ProfileImage = imageName;

                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                var path3 = string.Format("{0}\\{1}", pathString3, imageName);
                var path4 = string.Format("{0}\\{1}", pathString4, imageName);

                file.SaveAs(path3);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
                img.Save(path4);
            }
            return RedirectToAction("Doctors");
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

        public ActionResult DeleteDoctor(int id) { 

            int userId;
            //delete from database
            using (Db db = new Db())
            {
                DoctorDTO dto = db.Doctors.Find(id);
                userId = dto.UserId;
                UserDTO udto = db.Users.Find(userId);

                db.Doctors.Remove(dto);
                db.Users.Remove(udto);

                db.SaveChanges();
            }
            //delete project folders
            var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\Registration", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Doctors\\" + id.ToString());
            string pathString1 = Path.Combine(originalDirectory.ToString(), "Users\\" + userId.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);
            if (Directory.Exists(pathString1))
                Directory.Delete(pathString1, true);

            return RedirectToAction("Doctors");
        }

    }
}