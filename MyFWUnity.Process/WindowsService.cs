using Microsoft.Practices.Unity;
using MyFWUnity.Common;
using MyFWUnity.Common.Module;
using MyFWUnity.Common.Services;
using MyFWUnity.Core.Services;
using MyFWUnity.Process;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using Topshelf;

namespace MyFWUnity.Process
{
    public class WindowsService : ServiceControl, IDisposable
    {
        #region ServiceControl 成员
        protected ServiceHost _host = null;
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(HostControl hostControl)
        {
            try
            {
                var config = new HttpSelfHostConfiguration("http://localhost:"+ "Port".ConfigValue("8089")); //配置主机
                config.Routes.MapHttpRoute(
              name: "DefaultApi",
              routeTemplate: "api/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional }
          );

                if (!ApplicationService.Instance.IsInitialized)
                    ApplicationService.Instance.Initialize();

                IUnityContainer container = ServiceLocator.Instance.GetContainer();
                config.DependencyResolver= new Microsoft.Practices.Unity.WebApi.UnityDependencyResolver(container);
                config.Services.Replace(typeof(IAssembliesResolver), new WebApiResolver());
               
                using (HttpSelfHostServer server = new HttpSelfHostServer(config)) //监听HTTP
                {
                    server.OpenAsync().Wait(); //开启来自客户端的请求
                    Console.WriteLine("Press Enter to quit");
                    Console.ReadLine();
                }
                //_host = new ServiceHost(typeof(ProviderInterface));
                //if (_host.State != CommunicationState.Opening || _host.State != CommunicationState.Opened)
                //    _host.Open();
                //if ("IsRest".ConfigValue("false").ToLower() == "true") //是否启动 Rest 服务
                //{
                //    _webHost = new WebServiceHost(typeof(RestInterface), new Uri(_host.BaseAddresses[0].ToString() + "/Rest"));
                //    if (_webHost.State != CommunicationState.Opening || _webHost.State != CommunicationState.Opened)
                //        _webHost.Open();
                //    LogModule.Info("Rest 服务开启成功");
                //}
            }
            catch (Exception ex)
            {
                LogModule.Error("WindowsService->Start:" + ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            if (_host == null)
                return true;
            if (_host.State != CommunicationState.Closed)
            {
                try
                {
                    _host.Close();
                }
                catch (Exception ex)
                {
                    LogModule.Error("WindowsService->Stop:" + ex);
                    _host.Abort();
                }
            }
            //if ("IsRest".ConfigValue("false").ToLower() == "true" && _webHost != null && _webHost.State != CommunicationState.Closed)
            //{
            //    try
            //    {
            //        _webHost.Close();
            //        LogModule.Info("Rest 服务停止成功");
            //    }
            //    catch (Exception ex)
            //    {
            //        LogModule.Error("WindowsService Rest->Stop:" + ex);
            //        _webHost.Abort();
            //    }
            //}
            return true;
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            GC.Collect();
        }

        #endregion
    }
}
