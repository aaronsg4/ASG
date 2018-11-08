using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ASG
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "NewSlug",
                url: "Blog/{slug}",
                defaults: new
                {
                    controller = "BlogPosts",
                    action = "Details",
                    slug = UrlParameter.Optional
                });
            //routes.MapRoute(
            //    name: "Default",
            //    //url: "index.html"
            //    url: "{controller}/{action}/{id}",
            //    //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //    defaults: new { controller = "index.html", action = UrlParameter.Optional, id = UrlParameter.Optional }
            //);


            routes.MapRoute(
                name: "Default2",
                //url: "index.html"
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "ASG.Controllers" }

                //defaults: new { url = "index.html", id = UrlParameter.Optional }
                //defaults: new { controller = "index.html", action = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Default",
            url: "index.html"
            );
        }
    }
}
