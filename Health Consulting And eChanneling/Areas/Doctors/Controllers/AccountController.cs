using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Doctors.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Profile");
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
    }
}
