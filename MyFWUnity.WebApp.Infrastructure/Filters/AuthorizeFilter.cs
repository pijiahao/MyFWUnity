using MyFWUnity.Common.Module;
using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.WebApp.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Infrastructure.Filters
{
    public class AuthorizeFilter : AuthorizeAttribute
    {
        public AuthorizeFilter()
        {

        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Copy low level code
            //if (httpContext == null)
            //    throw new ArgumentNullException("httpContext");

            //Type baseType = typeof(AuthorizeAttribute);
            //FieldInfo userFI = baseType.GetField("_usersSplit", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic);
            //string[] _usersSplit = userFI.GetValue(this) as string[];

            //FieldInfo roleFI = baseType.GetField("_rolesSplit", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic);
            //string[] _rolesSplit = roleFI.GetValue(this) as string[];

            //IPrincipal user = httpContext.User;
            //bool result =  user.Identity.IsAuthenticated &&
            //    (_usersSplit.Length <= 0 || Enumerable.Contains<string>((IEnumerable<string>)_usersSplit, user.Identity.Name, (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)) &&
            //    (_rolesSplit.Length <= 0 || Enumerable.Any<string>((IEnumerable<string>)_rolesSplit, new Func<string, bool>(user.IsInRole)));

            bool result = base.AuthorizeCore(httpContext);
            return result;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            BaseController controller = filterContext.Controller as BaseController;
            if (controller != null)
            {
                string redirectUrl = filterContext.HttpContext.Request.RawUrl;
                IUserService uerService = ServiceHelper.GetService(typeof(IUserService)) as IUserService;
                UserDataInfo user = uerService.GetUserByID(controller.CurrentUser);
                if (user == null)
                {
                    filterContext.Result = new RedirectResult("/Admin/Account/Login?returnUrl=" + redirectUrl);
                }
                else
                {
                    base.OnAuthorization(filterContext);
                }

            }
            else
            {

                base.OnAuthorization(filterContext);
            }
        }
    }
}
