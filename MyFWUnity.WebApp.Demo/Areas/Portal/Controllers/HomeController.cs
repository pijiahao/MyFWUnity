using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Areas.Portal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult NewsList()
        {
            return View();
        }
        public ActionResult NewsDetail(string id)
        {
            return View();
        }

        public ActionResult Search(string searchText)
        {
            ViewBag.SearchText = searchText;
            return View();
        }

    }
}