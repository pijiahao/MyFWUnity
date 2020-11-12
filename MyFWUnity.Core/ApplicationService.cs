using MyFWUnity.Common.Config;
using MyFWUnity.Common.Module;
using MyFWUnity.Common.Services;
using MyFWUnity.Core.Infrastructure;
using MyFWUnity.Core.Infrastructure.Email;
using MyFWUnity.Core.Model;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Services
{
    public class ApplicationService
    {
        private static object mobjLock = new object();
        private static ApplicationService mobjInstance = new ApplicationService();
        public bool IsInitialized { get; set; }

        public static ApplicationService Instance
        {
            get
            {
                return mobjInstance;
            }
        }

        private ApplicationService()
        {
        }

        public void Initialize()
        {
            lock (mobjLock)
            {
                if (IsInitialized)
                    return;

                // Register all interfaces first
                IUnityContainer container = ServiceLocator.Instance.GetContainer();
                IEmailService emailService = ServiceLocator.Instance.GetService<IEmailService>();
                if (emailService != null)
                {
                    emailService.EmailServerConfig = (EmailServerSettingSection)ConfigurationManager.GetSection("Email");
                }
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    // fix bug that GetTypes() of some assembly may throw exception
                    string assebmlyName = assembly.GetName().Name.ToLower();
                    try
                    {
                        IEnumerable<Type> definedTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract);

                        RegisterRepositories(definedTypes);
                    }
                    catch (Exception ex)
                    {
                        LogModule.Error(string.Format("Failed to load dll {0}", assebmlyName), ex);
                    }
                }
                IsInitialized = true;
            }
        }
        private void RegisterRepositories(IEnumerable<Type> definedTypes)
        {
            IUnityContainer container = ServiceLocator.Instance.GetContainer();

            Type repositoryInterface = typeof(IRepository<>);
            Type entityRepositoryInterface = typeof(IEntityRepository);

            foreach (Type type in definedTypes)
            {
                Type[] parentIntefaces = type.GetInterfaces();

                // Is IRepository<T>
                if (IsGenericTypeOf(type, repositoryInterface))
                {
                    Type parentInterface = GetParentGenericInterface(repositoryInterface, parentIntefaces);
                    if (parentInterface != null)
                    {
                        LogModule.Debug(string.Format("Regsiter type {0} to interface {1}", type.FullName, parentInterface.FullName));
                        container.RegisterType(parentInterface, type);
                    }
                }

                Attribute[] customAttributes = Attribute.GetCustomAttributes(type, false);
                if (customAttributes != null)
                {
                    EntityRepositoryAttribute entityRepositoryAtt = customAttributes.FirstOrDefault(a => a is EntityRepositoryAttribute) as EntityRepositoryAttribute;
                    if (entityRepositoryAtt != null)
                    {
                        string name = entityRepositoryAtt.EntityClassName;
                        if (!string.IsNullOrEmpty(entityRepositoryAtt.EntityClassName))
                        {
                            // Is IEntityRepository
                            if (parentIntefaces.Any(t => t == entityRepositoryInterface))
                            {
                                LogModule.Debug(string.Format("Regsiter type {0} to interface {1}", type.FullName, entityRepositoryInterface.FullName));
                                container.RegisterType(entityRepositoryInterface, type, name);
                            }
                        }
                    }
                }
            }
        }
        private Type GetParentGenericInterface(Type repositoryInterface, Type[] interfaces)
        {
            if (null == interfaces || 0 == interfaces.Count())
            {
                return null;
            }

            foreach (var type in interfaces)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == repositoryInterface.GetGenericTypeDefinition())
                {
                    continue;
                }
                if (IsGenericTypeOf(type, repositoryInterface))
                {
                    return type;
                }
            }
            return null;
        }

        private bool IsGenericTypeOf(Type type, Type genericDefinition)
        {
            Type[] parameters = null;
            return IsGenericTypeOf(type, genericDefinition, out parameters);
        }

        private bool IsGenericTypeOf(Type type, Type genericDefinition, out Type[] genericParameters)
        {
            genericParameters = new Type[] { };
            if (!genericDefinition.IsGenericType)
            {
                return false;
            }

            var isMatch = type.IsGenericType && type.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();
            if (!isMatch && type.BaseType != null)
            {
                isMatch = IsGenericTypeOf(type.BaseType, genericDefinition, out genericParameters);
            }
            if (!isMatch && genericDefinition.IsInterface && type.GetInterfaces().Any())
            {
                foreach (var i in type.GetInterfaces())
                {
                    if (IsGenericTypeOf(i, genericDefinition, out genericParameters))
                    {
                        isMatch = true;
                        break;
                    }
                }
            }

            if (isMatch && !genericParameters.Any())
            {
                genericParameters = type.GetGenericArguments();
            }
            return isMatch;
        }

    }
}
