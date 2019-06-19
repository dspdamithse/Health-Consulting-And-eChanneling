using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                if (db.SpecialistArea.Any(x => x.SpecialistArea == catName))
                    return "titletaken";

                SpecialistAreaDTO dto = new SpecialistAreaDTO();

                dto.SpecialistArea = catName;
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
                if (db.SpecialistArea.Any(x => x.SpecialistArea == newCatName))
                    return "titletaken";

                SpecialistAreaDTO dto = db.SpecialistArea.Find(id);

                dto.SpecialistArea = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                db.SaveChanges();
            }
            return "Ok";
        }

    }
}