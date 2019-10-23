using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Account;
using Health_Consulting_And_eChanneling.Models.ViewModels.Doctors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    public class UsersController : Controller
    {
        // GET: Administrator/Users
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Users()
        {
            List<UserViewModel> userList;

            using (Db db = new Db())
            {
                userList = db.Users.ToArray().OrderByDescending(x => x.Id).Where(x=>x.RoleConfirm== 0).Where(x=>x.Id != 1).Select(x => new UserViewModel(x)).ToList();
            }

            //Return view with list
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

                model.MedicalImages = Directory.EnumerateFiles(Server.MapPath("~/Content/MedicalImages/1/"))
                                               .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }
    }
}