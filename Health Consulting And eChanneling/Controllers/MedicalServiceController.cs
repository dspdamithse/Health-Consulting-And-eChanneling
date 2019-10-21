using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.MediService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Controllers
{
    [RoutePrefix("medical-services")]
    public class MedicalServiceController : Controller
    {
        // GET: MedicalService
        [Route("all-services")]
        public ActionResult MedicalServiceList()
        {
            List<MediServiceViewModel> mediServiceList;

            using (Db db = new Db())
            {
                mediServiceList = db.MediService.ToArray().OrderByDescending(x => x.Id).Select(x => new MediServiceViewModel(x)).ToList();
            }

            return View("MedicalServiceList", mediServiceList);
        }
    }
}