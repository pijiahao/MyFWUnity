using MyFWUnity.Common;
using MyFWUnity.Common.Module;
using MyFWUnity.WebApp.Infrastructure.Model.ResultData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Infrastructure.Utilities.ExceptionHandler
{
    public class MvcControllerHandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        public override void OnException(System.Web.Mvc.ExceptionContext filterContext)
        {
            StringBuilder lobjLogBuilder = new StringBuilder();
            lobjLogBuilder.Append("Error occurred in controller action.");
            lobjLogBuilder.Append(string.Format("Controller-{0};", filterContext.RouteData.Values["controller"]));
            lobjLogBuilder.Append(string.Format("Action-{0};", filterContext.RouteData.Values["action"]));
            LogModule.Error(lobjLogBuilder.ToString(), filterContext.Exception);

            filterContext.ExceptionHandled = true;
            MessageModel msgModel = new MessageModel(MessageType.Error, ExceptionHelper.GetMessage(filterContext.Exception));
            ResultDataBag lobjResult = new ResultDataBag(true, msgModel, null);
            filterContext.Result = new JsonResult() { Data = lobjResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            base.OnException(filterContext);
        }
    }
}
