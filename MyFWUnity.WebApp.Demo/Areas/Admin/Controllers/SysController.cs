using MyFWUnity.WebApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Areas.Admin.Controllers
{
    public class SysController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Log()
        {
            return View();
        }
        public ActionResult Email()
        {
            return View();
        }

        public ActionResult Slide()
        {
            return View();
        }
    }
}