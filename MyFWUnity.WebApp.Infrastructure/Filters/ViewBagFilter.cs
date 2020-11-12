using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Infrastructure.Filters
{
    public class ViewBagFilterAttribute : System.Web.Mvc.FilterAttribute, IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            BaseController controller = filterContext.Controller as BaseController;
            if (controller != null)
            {
                controller.UpdateViewBag();
            }
        }


    }
}
