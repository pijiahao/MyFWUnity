using MyFWUnity.Common.Module;
using MyFWUnity.Common.Services;
using MyFWUnity.Core.Infrastructure.DatabaseContext;
using MyFWUnity.Core.Services;
using MyFWUnity.WebApp.Infrastructure.Application.Config;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace MyFWUnity.WebApp.Infrastructure.Application
{
    public class CustomRazorViewEngine : RazorViewEngine
    {
        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            ViewLocationFormats = new string[] { "~/Areas/Portal/Views/{1}/{0}.cshtml", "~/Views/{1}/{0}.cshtml" }; ;
            return base.FindView(controllerContext, viewName, masterName, false);
        }


    }
    public abstract class BaseMvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RegisterAreas();

            if (!ApplicationService.Instance.IsInitialized)
                ApplicationService.Instance.Initialize();

            IUnityContainer container = ServiceLocator.Instance.GetContainer();

            // WebApi
            GlobalConfiguration.Configuration.DependencyResolver = new Microsoft.Practices.Unity.WebApi.UnityDependencyResolver(container);

            // MVC
            DependencyResolver.SetResolver(new Microsoft.Practices.Unity.Mvc.UnityDependencyResolver(container));


            //IEmailService emailService = ServiceHelper.GetEmailService();
            //if (emailService != null)
            //{
            //    emailService.EmailServerConfig = (EmailServerSettingSection)ConfigurationManager.GetSection("Email");
            //}

            //InitializeMvcFactory();

            RegisterApiConfig();
            RegisterGlobalFilter();
            RegisterRoutes();
            RegisterBundles();

            //路由注册
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new CustomRazorViewEngine());
        }

        protected virtual void RegisterAreas()
        {
            AreaRegistration.RegisterAllAreas();
        }
            
        protected virtual void RegisterBundles()
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected virtual void RegisterRoutes()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected virtual void RegisterGlobalFilter()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        protected virtual void RegisterApiConfig()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }


        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);

            base.Init();

        }

        /// <summary>
        /// Get the EF DB Context when begin the Http Request so that the application could use one context per request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //Avoid Error("Response for preflight has invalid HTTP status code 405") from H5 App. By zengjun.
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.End();
            }
        }

        /// <summary>
        /// Dispose the EF DB Context when begin the Http Request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            try
            {
                // Singleton
                IEnumerable<IRepositoryContext> lcolContexts = ServiceLocator.Instance.GetServices<IRepositoryContext>();
                if (lcolContexts != null)
                {
                    foreach (IRepositoryContext lobjContext in lcolContexts)
                    {
                        LogModule.Debug("Start to dispose DB context");
                        lobjContext.Dispose();
                        LogModule.Debug("Disposed DB context");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                LogModule.Error("Failed to dispose DB Context", ex);
            }
        }
    }
}
