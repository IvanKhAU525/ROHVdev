using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ROHV
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;

            routes.MapRoute(
                name: "Templates",
                url: "template/{controller}/{action}",
                defaults: new { controller = "Consumers", action = "Index" }
            );
            /*routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );*/
            routes.MapRoute(
                name: "DefaultApi",
                url: "api/{controller}/{action}/{id}",
                defaults: new { id = UrlParameter.Optional }
          );
            routes.MapRoute(
               name: "Account",
               url: "account/{action}/{id}",
               defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                "AllPages",
                "{*url}",
                new { controller = "Home", action = "Index" }
            );



        }
    }
}
