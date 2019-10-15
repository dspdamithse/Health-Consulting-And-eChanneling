using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Controllers
{
    [RoutePrefix("important-news")]
    public class ImportantNewsController : Controller
    {
        [Route("{slug}")]
        public ActionResult Index(string slug)
        {
            NewsViewModel model;
            NewsDTO dto;
            using (Db db = new Db())
            {
                if (!db.News.Any(x => x.Slug.Equals(slug)))
                {
                    return RedirectToAction("Index", new { slug = "first-slug" });
                }
            }

            using (Db db = new Db())
            {
                dto = db.News.Where(x => x.Slug == slug).FirstOrDefault();
            }
            
            model = new NewsViewModel(dto);
            return View(model);
        }
        
        public ActionResult NewsListMenuView()
        {
            List<NewsViewModel> newsViewModelList;

            using (Db db = new Db())
            {
                newsViewModelList = db.News.ToArray().OrderBy(x => x.Id)
                                                      .Select(x => new NewsViewModel(x))
                                                      .ToList();

            }
            return PartialView(newsViewModelList);
        }
        [Route("all-news-articals")]
        public ActionResult NewsList()
        {
            List<NewsViewModel> newsList;

            using (Db db = new Db())
            {
                newsList = db.News.ToArray().OrderBy(x => x.Id).Select(x => new NewsViewModel(x)).ToList();
            }

            //Return view with list
            return View("NewsList", newsList);
        }
    }
}