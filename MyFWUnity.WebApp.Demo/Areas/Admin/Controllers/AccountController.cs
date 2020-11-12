using MyFWUnity.Common.Encrypt;
using MyFWUnity.Common.Integration.Interfaces;
using MyFWUnity.Common.Module;
using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.WebApp.Infrastructure.Model.User;
using MyFWUnity.WebApp.Infrastructure.Utilities;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MyFWUnity.WebApp.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        [Dependency]
        public IUserService UserService { get; set; }

        [Dependency]
        public ILoginInfoPersistenceService LoginInfoPersistenceService { get; set; }

        //
        // GET: /Account/

        [HttpGet]
        public virtual ActionResult Login(string returnUrl)
        {
            UserService.CreateAdminUser();
            try
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            catch (Exception ex)
            {
                LogModule.Error(ex.Message);
            }

            return View();
        }


        //
        // POST: /Account/Login
        [HttpPost]
        public virtual ActionResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserCode) || string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("", "请输入用户名或密码。");
                    return View(model);
                }
                string password = EncryptManager.Encode(model.Password);
                UserDataInfo user = UserService.GetUserDataInfoByLogin(model.UserCode, password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.UserCode, false);
                    LoginInfoPersistenceService.SaveLoginUser(user.ID);             
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect("/Admin/Sys/Slide");
                    }
                    else
                    {
                        if (returnUrl.Equals("/"))
                        {
                            return Redirect("/Admin/Sys/Slide");
                        }
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码错误");
                }

            }
            catch (Exception ex)
            {
                LogModule.Error("Failed to log in.");
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            LoginInfoPersistenceService.DeleteUser();
            return RedirectToAction("Login", "Account");

        }
    }
}