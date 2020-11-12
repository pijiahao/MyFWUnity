using MyFWUnity.Common.Module;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common.Config
{
    public class CommonDefine
    {
        private const string LogPath = "Log";
        private const string LogLevel = "LogLevel";
        private const string DefaultPage = "DefaultPage";
        private const string IsRefreshMenuOrPermission = "IsRefreshMenuOrPermission";
        private const string DefaultUserPassword = "DefaultUserPassword";
        public static string GetLogPath()
        {
            string folder = System.AppDomain.CurrentDomain.BaseDirectory + LogPath;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }
        public static bool GetIsRefreshMenuOrPermission()
        {
            bool result = false;
            bool.TryParse(IsRefreshMenuOrPermission.ConfigValue(), out result);
            return result;
        }

        public static int GetLogLevel()
        {
            int level = 0;
            if (!int.TryParse(LogLevel.ConfigValue(), out level))
            {
                level = 1;
            }
            return level;
        }
        public static string GetDefaultPage()
        {
            string defaultPage = string.Empty;
            try
            {
                defaultPage = DefaultPage.ConfigValue();
            }
            catch (Exception ex)
            {
                LogModule.Error(ex.Message, ex);
            }
            return defaultPage;
        }
        public static string GetDefaultUserPassword()
        {
            return DefaultUserPassword.ConfigValue("123456");
        }
    }
}
