using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Health_Consulting_And_eChanneling.Controllers
{
    [RoutePrefix("our-doctors")]
    public class FrontDoctorsController : Controller
    {
        // GET: FrontDoctors
        [Route("all-doctors")]
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
                ViewBag.SpecialistArea = new SelectList(db.DoctorSpecialist.ToList(), "Id", "Name");
                ViewBag.SelectedCat = catId.ToString();
            }
            var onePageOfProducts = listOfDoctorVM.ToPagedList(pageNumber, 10);

            ViewBag.OnePageOfProducts = onePageOfProducts;
            return View("Doctors", listOfDoctorVM);

        }
    }
}