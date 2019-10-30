using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoctorSpecialistController : Controller
    {
        // GET: Administrator/DoctorSpecialist
        [ActionName("all-doctor-specialist-categories")]
        public ActionResult SpecialistCategory()
        {
            List<DoctorSpecialistViewModel> DoctorSpecialistViewModelList;

            using (Db db = new Db())
            {
                DoctorSpecialistViewModelList = db.DoctorSpecialist
                                .ToArray()
                                .OrderBy(x => x.Sorting)
                                .Select(x => new DoctorSpecialistViewModel(x))
                                .ToList();
            }
            return View("SpecialistCategory", DoctorSpecialistViewModelList);
        }

        [HttpPost]
        public string AddNewDoctorSpecialistCategory(string catName)
        {
            string id;
            using (Db db = new Db())
            {
                if (db.DoctorSpecialist.Any(x => x.Name == catName))
                    return "titletaken";

                DoctorSpecialistDTO dto = new DoctorSpecialistDTO();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.DoctorSpecialist.Add(dto);
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
                DoctorSpecialistDTO dto;
                foreach (var catId in id)
                {
                    dto = db.DoctorSpecialist.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }

        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                DoctorSpecialistDTO dto = db.DoctorSpecialist.Find(id);

                db.DoctorSpecialist.Remove(dto);

                db.SaveChanges();
            }
            return RedirectToAction("all-doctor-specialist-categories");
        }

        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.DoctorSpecialist.Any(x => x.Name == newCatName))
                    return "titletaken";

                DoctorSpecialistDTO dto = db.DoctorSpecialist.Find(id);

                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                db.SaveChanges();
            }
            return "Ok";
        }

    }
}