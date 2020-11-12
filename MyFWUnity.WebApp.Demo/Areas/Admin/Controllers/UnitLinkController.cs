using MyFWUnity.WebApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Areas.Admin.Controllers
{
    public class UnitLinkController : BaseController
    {
        // GET: Admin/UnitLink
        public ActionResult Index()
        {
            return View();
        }
    }
}