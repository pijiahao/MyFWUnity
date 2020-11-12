using MyFWUnity.Common;
using MyFWUnity.Common.Module;
using System;
using Topshelf;

namespace MyFWUnity.Process
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                HostFactory.Run(o =>
                {
                    o.Service<WindowsService>();
                    o.RunAsLocalSystem();
                    o.EnablePauseAndContinue();
                    o.StartAutomatically();
                    o.SetDescription(string.Format("MyFWUnity WebApi 服务.{0}", "ServicesDescription".ConfigValue("")));
                    o.SetDisplayName(string.Format("MyFWUnity.Process_{0}.exe", "Port".ConfigValue("8080")));
                    o.SetServiceName(string.Format("Conlin.Enterprise.Service.Bus_{0}", "Port".ConfigValue("8080")));
                });
            }
            catch (Exception ex)
            {
                LogModule.Error("MyFWUnity框架异常:" + ex);
            }
        }
    }
}
