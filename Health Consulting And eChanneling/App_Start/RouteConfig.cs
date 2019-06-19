using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Health_Consulting_And_eChanneling
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial",  new { controller = "Pages", action = "PagesMenuPartial" }, new [] { "Health_Consulting_And_eChanneling.Controllers" });
            routes.MapRoute("Pages","{page}",  new { controller = "Pages", action = "Index" }, new [] { "Health_Consulting_And_eChanneling.Controllers" });
            routes.MapRoute("Default","",  new { controller = "Pages", action = "Index" }, new [] { "Health_Consulting_And_eChanneling.Controllers" });
            /*routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );*/
        }
    }
}
