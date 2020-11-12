using MyFWUnity.Common.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyFWUnity.WebApp.Infrastructure.Application.Config
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            string defaultsAppSettings = CommonDefine.GetDefaultPage();
            string[] defaultArray = defaultsAppSettings.Split('/');

            routes.MapRoute(
                name: "Default",
                url: "{PageType}/{controller}/{action}/{id}",
               namespaces: new string[] { "MyFWUnity.WebApp.Areas.Portal.Controllers" },
                defaults: new { PageType = "Portal", controller = defaultArray[0], action = defaultArray[1], id = UrlParameter.Optional }
            );
        }
    }
}
