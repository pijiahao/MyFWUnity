using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyFWUnity.Common
{
    public class ApplicationHelper
    {
        public static object Get(string key)
        {
            object obj = null;
            if (HttpContext.Current.Application[key] != null)
            {
                obj = HttpContext.Current.Application[key];
            }
            return obj;
        }

        public static bool Has(string key)
        {
            return HttpContext.Current.Application.AllKeys.Contains(key);

        }


        public static void Set(string key, object obj)
        {
            HttpContext.Current.Application[key] = obj;
        }

        public static void Remove(string key)
        {
            HttpContext.Current.Application.Remove(key);
        }

        public static void Clear()
        {
            HttpContext.Current.Application.Clear();
        }
    }
}
