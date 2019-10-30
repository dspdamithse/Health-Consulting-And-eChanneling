using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Doctors.Controllers
{
    public class UserMedicalHostoryController : Controller
    {
        // GET: Doctors/UserMedicalHostory
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
        public ActionResult Details(int Id)
        {

            int id = 0;
            UserProfileViewModel model;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Id == Id);
                if (dto == null)
                {
                    return Content("User is not available");
                }
                id = dto.Id;

                model = new UserProfileViewModel(dto);
            }

            //Medical Records
            model.MedicalImages = Directory.EnumerateFiles(Server.MapPath("~/Content/MedicalImages/" + id + "/"))
                                               .Select(fn => Path.GetFileName(fn));
            return View("Details", model);
        }
    }
}