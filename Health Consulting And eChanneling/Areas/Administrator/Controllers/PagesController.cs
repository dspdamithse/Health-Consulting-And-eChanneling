using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    public class PagesController : Controller
    {
        // GET: Administrator/Pages
        public ActionResult Index()
        {
            //Declare list of PageViewModels
            List<PageViewModel> pagesList;
          
            using (Db db = new Db())
            {
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageViewModel(x)).ToList();
            }

            //Return view with list
            return View(pagesList);
        }
    }
}