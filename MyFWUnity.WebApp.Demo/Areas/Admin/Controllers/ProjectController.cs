using MyFWUnity.WebApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Areas.Admin.Controllers
{
    public class ProjectController : BaseController
    {
        // GET: Project
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserManager() {
            return View();
        }
    }
}