using MyFWUnity.Common.Module;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common.Services
{
    public class ServiceLocator : IServiceProvider
    {
        private readonly IUnityContainer mobjContainer = null;
        private static readonly ServiceLocator instance = new ServiceLocator();

        private ServiceLocator()
        {
            UnityConfigurationSection section = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;
            mobjContainer = new UnityContainer();
            section.Configure(mobjContainer, "Default");
        }

        public static ServiceLocator Instance
        {
            get { return instance; }
        }

        public IUnityContainer GetContainer()
        {
            return mobjContainer;
        }

        public object GetService(Type serviceType)
        {
            if (IsRegistered(serviceType))
            {
                return mobjContainer.Resolve(serviceType);
            }
            else
            {
                LogModule.Warn(string.Format("Service type {0} is not registered", serviceType.ToString()));
                return null;
            }
        }

        public object GetService(Type serviceType, string name)
        {
            if (IsRegistered(serviceType, name))
            {
                return mobjContainer.Resolve(serviceType, name);
            }
            else
            {
                LogModule.Warn(string.Format("Service type {0} is not registered with name {1}", serviceType.ToString(), name));
                return null;
            }
        }


        public T GetService<T>()
        {
            if (IsRegistered<T>())
            {
                return mobjContainer.Resolve<T>();
            }
            else
            {
                Type type = typeof(T);
                LogModule.Warn(string.Format("Service type {0} is not registered", type.ToString()));
                return default(T);
            }
        }

        public T GetService<T>(string name)
        {
            if (IsRegistered<T>(name))
            {
                return mobjContainer.Resolve<T>(name);
            }
            else
            {
                Type type = typeof(T);
                LogModule.Warn(string.Format("Service type {0} is not registered with name {1}", type.ToString(), name));
                return default(T);
            }
        }

        public IEnumerable<T> GetServices<T>()
        {
            return mobjContainer.ResolveAll<T>();
        }

        private bool IsRegistered(Type serviceType, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                return mobjContainer.IsRegistered(serviceType);
            }
            else
            {
                return mobjContainer.IsRegistered(serviceType, name);
            }
        }

        private bool IsRegistered<T>(string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                return mobjContainer.IsRegistered<T>();
            }
            else
            {
                return mobjContainer.IsRegistered<T>(name);
            }
        }
    }
}
