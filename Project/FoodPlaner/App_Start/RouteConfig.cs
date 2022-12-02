using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FoodPlaner
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RecipeIndex",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Recipe", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RecipeShow",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Recipe", action = "Show", id = UrlParameter.Optional }
            );
        }
    }
}
