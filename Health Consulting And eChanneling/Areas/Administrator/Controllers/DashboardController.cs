using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Areas.Administrator.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Administrator/Dashboard
        public ActionResult Index()
        {
            return View();
        }
    }
}