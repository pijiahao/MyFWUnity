using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Areas.Portal.Controllers
{
    public class AccountController : Controller
    {
        // GET: Portal/Account
        public ActionResult Login()
        {
            return View();
        }
    }
}