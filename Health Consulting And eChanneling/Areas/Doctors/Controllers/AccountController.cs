using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Doctors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Doctors.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("doctor-profile");
        }
        [ActionName("doctor-profile")]
        public ActionResult DoctorProfile()
        {
            string username = User.Identity.Name;

            DoctorViewModel model;

            int id = 0;
            using (Db db = new Db())
            {
                DoctorDTO dto = db.Doctors.FirstOrDefault(x => x.Username == username);
                id = dto.Id;

                model = new DoctorViewModel(dto);
            }

            return View("DoctorProfile", model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
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
        public ActionResult Edit(DoctorViewModel model, HttpPostedFileBase file)
        {
            int id = model.Id;
            int userid = model.UserId;
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
                udto.RoleConfirm = 1;

                db.SaveChanges();
            }
            using (Db db = new Db())
            {
                DoctorDTO ddto = db.Doctors.Find(id);
                ddto.FirstName = model.FirstName;
                ddto.LastName = model.LastName;
                ddto.EmailAddress = model.EmailAddress;
                ddto.SLMC_Reg_No = model.SLMC_Reg_No;
                ddto.ContactNumber = model.ContactNumber;
                ddto.About = model.About;
                ddto.SpecialistAreaId = model.SpecialistAreaId;

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
            return RedirectToAction("doctor-profile");
        }

        [HttpGet]
        public ActionResult ResetPassword(int id)
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
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ResetPassword(DoctorViewModel model)
        {
            if (model.Password.Length>0)
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password and Confirm password are not match");
                    return RedirectToAction("ResetPassword", model);
                }
            }
            
            int id = model.Id;
            int userid = model.UserId;


            using (Db db = new Db())
            {
                UserDTO udto = db.Users.Find(userid);
                udto.Password = model.Password;

                db.SaveChanges();
            }
            using (Db db = new Db())
            {
                DoctorDTO ddto = db.Doctors.Find(id);
                ddto.Password = model.Password;

                db.SaveChanges();
            }

            TempData["SM"] = "Successfully Updated the password";

            return RedirectToAction("doctor-profile");
        }
    }
}
