using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Account;
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
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Users");
        }
        public ActionResult Users()
        {
            List<UserViewModel> userList;

            using (Db db = new Db())
            {
                userList = db.Users.ToArray().OrderByDescending(x => x.Id).Where(x => x.RoleConfirm == 0).Where(x => x.Id != 1).Select(x => new UserViewModel(x)).ToList();
            }

            return View(userList);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            UserProfileViewModel model;
            using (Db db = new Db())
            {
                UserDTO dto = db.Users.Find(id);
                if (dto == null)
                {
                    return Content("Product is not available");
                }
                model = new UserProfileViewModel(dto);

                model.MedicalImages = Directory.EnumerateFiles(Server.MapPath("~/Content/MedicalImages/" + id + "/"))
                                               .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }
        [HttpPost]
        public void SaveMedicalImages(int id)
        {
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                if (file != null && file.ContentLength > 0)
                {
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\MedicalImages", Server.MapPath(@"\")));

                    string pathString = Path.Combine(originalDirectory.ToString(), id.ToString());

                    var path = string.Format("{0}\\{1}", pathString, file.FileName);


                    file.SaveAs(path);
                }
            }   
        }

        public void DeleteImage(int id, string imageName)
        {
            string fullpath1 = Request.MapPath("~/Content/MedicalImages" + id.ToString() +"/"+ imageName);

            if (System.IO.File.Exists(fullpath1))
                System.IO.File.Delete(fullpath1);

        }
    }
}