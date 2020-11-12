using MyFWUnity.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.WebApp.Infrastructure.Utilities
{
    public class ServiceHelper
    {
        public static object GetService(Type serviceType)
        {
            return ServiceLocator.Instance.GetService(serviceType);
        }
    }
}
