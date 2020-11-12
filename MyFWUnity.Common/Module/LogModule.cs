using MyFWUnity.Common.Config;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MyFWUnity.Common.Module
{
    public class LogModule
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        public static string CONFIGFILENAME = @"Log.xml";
        /// <summary>
        /// 配置文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetConfigFileName()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "\\" + CONFIGFILENAME;
        }
        /// <summary>
        /// 节点名称
        /// </summary> 
        public static string LOGCONFIGTAG = @"log4net";
        public static string DELLOGEXECUT = "";
        public static ConcurrentDictionary<String, String> _PATH = new ConcurrentDictionary<String, String>();
        /// <summary>
        /// 构造函数
        /// </summary>
        static LogModule()
        {
            XmlDocument doc = new XmlDocument();
            string sXmlConfig = GetConfigFileName();
            if (File.Exists(sXmlConfig))
            {
                doc.Load(sXmlConfig);
                XmlNode node = doc.SelectSingleNode("//" + LOGCONFIGTAG);

                if (node != null)
                    log4net.Config.XmlConfigurator.Configure((XmlElement)node);
                else
                    log4net.Config.XmlConfigurator.Configure();

                var _AppenderList = XElement.Parse(node.OuterXml).Elements();
                if (_AppenderList.Where(w => w.FirstAttribute.Value == "infoAppender").Count() > 0)
                    _PATH.AddOrUpdate("INFO", _AppenderList.Where(w => w.FirstAttribute.Value == "infoAppender").FirstOrDefault().Elements().FirstOrDefault().LastAttribute.Value, (key, value) => value);
                if (_AppenderList.Where(w => w.FirstAttribute.Value == "warnAppender").Count() > 0)
                    _PATH.AddOrUpdate("WARN", _AppenderList.Where(w => w.FirstAttribute.Value == "warnAppender").FirstOrDefault().Elements().FirstOrDefault().LastAttribute.Value, (key, value) => value);
                if (_AppenderList.Where(w => w.FirstAttribute.Value == "debugAppender").Count() > 0)
                    _PATH.AddOrUpdate("DEBUG", _AppenderList.Where(w => w.FirstAttribute.Value == "debugAppender").FirstOrDefault().Elements().FirstOrDefault().LastAttribute.Value, (key, value) => value);
            }
            else
                log4net.Config.XmlConfigurator.Configure();
        }
        public static void LogFileDetele()
        {
            try
            {
                if (_PATH.Count == 0) return;
                if (DELLOGEXECUT == DateTime.Now.ToString("yyyyMMdd")) return;
                //异步删除保留天数外的日志
                Task.Run(() =>
                {
                    _PATH.ToList().ForEach(o =>
                    {
                        DelLog(o.Value);
                    });
                    DELLOGEXECUT = DateTime.Now.ToString("yyyyMMdd");
                });
            }
            catch (Exception ex)
            {
                Error("LogModule->LogFileDetele:删除日志失败:" + ex.InnerException());
            }
        }

        private static void DelLog(string v)
        {
            try
            {
                var _Months = Convert.ToInt32("KeepLogMonths".ConfigValue("-1"));
                //跨月的删除保留月份以前的
                if (Directory.Exists(v + DateTime.Now.AddMonths(_Months).ToString("yyyy-MM")))
                {
                    var files = Directory.GetFiles(v + DateTime.Now.AddMonths(_Months).ToString("yyyy-MM"), "*.log.*");
                    files.ToList().ForEach(o => File.Delete(o));
                }
                //当月的删除保留天数以前的
                if (_Months == -1)
                    DelLogWithDay(v);
            }
            catch (Exception ex)
            {
                Error("LogModule->LogModule:删除日志失败:" + ex.InnerException());
            }
        }

        private static void DelLogWithDay(string v)
        {
            var _Days = Convert.ToInt32("KeepLogDays".ConfigValue("-7"));
            if (Directory.Exists(v + DateTime.Now.AddDays(_Days).ToString("yyyy-MM")))
            {
                var files = Directory.GetFiles(v + DateTime.Now.ToString("yyyy-MM"), "*.log.*");
                files.Where(w => Convert.ToDateTime(w.Substring((v + DateTime.Now.ToString("yyyy-MM") + "\\").Length, 10)).Date <= DateTime.Now.AddDays(_Days).Date).ToList().ForEach(o =>
                {
                    File.Delete(o);
                });
            }
        }

        /// <summary>
        /// 一般信息.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logger"></param>
        public static void Info(object message, string logger = "loginfo")
        {
            ILog log = LogManager.GetLogger(logger);
            log.Info(message);
            LogFileDetele();
        }
        /// <summary>
        /// 一般信息.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="logger"></param>
        public static void Info(object message, Exception exception, string logger = "loginfo")
        {
            ILog log = LogManager.GetLogger(logger);
            log.Info(message, exception);
            LogFileDetele();
        }
        /// <summary>
        /// 一般错误.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logger"></param>
        public static void Error(object message, string logger = "logerror")
        {
            ILog log = LogManager.GetLogger(logger);
            log.Error(message);
        }
        /// <summary>
        /// 一般错误.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="logger"></param>
        public static void Error(object message, Exception exception, string logger = "logerror")
        {
            ILog log = LogManager.GetLogger(logger);
            log.Error(message, exception);
        }
        /// <summary>
        /// 警告.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logger"></param>
        public static void Warn(object message, string logger = "logwarn")
        {
            ILog log = LogManager.GetLogger(logger);
            log.Warn(message);
            LogFileDetele();
        }
        /// <summary>
        /// 警告.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="logger"></param>
        public static void Warn(object message, Exception exception, string logger = "logwarn")
        {
            ILog log = LogManager.GetLogger(logger);
            log.Warn(message, exception);
            LogFileDetele();
        }
        /// <summary>
        /// 调试信息.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logger"></param>
        public static void Debug(object message, string logger = "logdebug")
        {
            ILog log = LogManager.GetLogger(logger);
            if (log.IsDebugEnabled)
                log.Debug(message);
            LogFileDetele();
        }
        /// <summary>
        /// 调试信息.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="logger"></param>
        public static void Debug(object message, Exception exception, string logger = "logdebug")
        {
            ILog log = LogManager.GetLogger(logger);
            if (log.IsDebugEnabled)
                log.Debug(message, exception);
            LogFileDetele();
        }
        /// <summary>
        /// 致命错误.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logger"></param>
        public static void Fatal(object message, string logger = "logfatal")
        {
            ILog log = LogManager.GetLogger(logger);
            log.Fatal(message);
        }
        /// <summary>
        /// 致命错误.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="logger"></param>
        public static void Fatal(object message, Exception exception, string logger = "logfatal")
        {
            ILog log = LogManager.GetLogger(logger);
            log.Fatal(message, exception);
        }
    }
}
