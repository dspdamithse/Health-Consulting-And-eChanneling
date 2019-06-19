using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        public ActionResult Index(string page="")
        {
            if (page == "")
                page = "home";

            PageViewModel model;
            PageDTO dto;
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x=>x.Slug.Equals(page))) {
                    return RedirectToAction("Index", new { page=""});
                }
            }

            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }
            ViewBag.PageTitle = dto.Title;
            if (dto.HasSidebar==true)
            {
                ViewBag.HasSidebar = "Yes";
            }
            else
            {
                ViewBag.HasSidebar = "No";
            }
            model = new PageViewModel(dto);
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            List<PageViewModel> pageViewModelList;

            using (Db db=new Db())
            {
                pageViewModelList = db.Pages.ToArray().OrderBy(x => x.Sorting)
                                                      .Where(x => x.Slug != "home")
                                                      .Select(x=>new PageViewModel(x))
                                                      .ToList();

            }
            return PartialView(pageViewModelList);
        }
    }
}