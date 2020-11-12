using MyFWUnity.Common.Email;
using MyFWUnity.Common.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MyFWUnity.Common
{
    public static class ToolsEx
    {
        /// <summary>
        /// 对象2Xml
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXml(this object obj)
        {
            return XmlModule.Serializer(obj);
        }

        /// <summary>
        /// 对象2Xml
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="type">对象的类型</param>
        /// <param name="extraTypes">一个 System.Type 的其他对象类型进行序列化的数组</param>
        /// <returns></returns>
        public static string ToXml(this object obj, Type type, Type[] extraTypes)
        {
            return XmlModule.Serializer(obj, type, extraTypes);
        }

        /// <summary>
        /// Xml2对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T X2Entity<T>(this string obj) where T : class
        {
            return XmlModule.Deserialize(typeof(T), obj) as T;
        }

        /// <summary>
        /// 对象X2Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">对象</param>
        /// <param name="type">对象的类型</param>
        /// <param name="extraTypes">一个 System.Type 的其他对象类型进行序列化的数组</param>
        /// <returns></returns>
        public static T X2Entity<T>(this string obj, Type[] extraTypes) where T : class
        {
            return XmlModule.Deserialize(typeof(T), obj, extraTypes) as T;
        }


        /// <summary>
        /// 对象2Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            if (obj == null) return string.Empty;
            return JsonModule.Serialize(obj);
        }

        /// <summary>
        /// Json2对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T J2Entity<T>(this string obj)
        {
            return JsonModule.Deserialize<T>(obj);
        }


        /// <summary>
        /// 获取配置文件参数
        /// </summary>
        /// <param name="varName">参数名称</param>
        /// <returns></returns>
        public static string ConfigValue(this string varName)
        {
            string outVar = "";
            try
            {
                outVar = System.Configuration.ConfigurationManager.AppSettings[varName];
                if (string.IsNullOrEmpty(outVar))
                {
                    outVar = "";
                }
                return outVar;
            }
            catch
            {
                return outVar;
            }
        }

        /// <summary>
        /// 获取配置文件参数
        /// </summary>
        /// <param name="varName">参数名称</param>
        /// <param name="initValue">初始值</param>
        /// <returns></returns>
        public static string ConfigValue(this string varName, string initValue)
        {
            string outVar = "";
            try
            {

                outVar = System.Configuration.ConfigurationManager.AppSettings[varName];
                if (string.IsNullOrEmpty(outVar))
                {
                    outVar = initValue;
                }
                return outVar;
            }
            catch
            {
                return initValue;
            }
        }
        /// <summary>
        /// 校验参数
        /// </summary>
        /// <param name="varValue"></param>
        /// <param name="varName"></param>
        public static void IsNullOrEmptyOfVar(this string varValue, string varName)
        {
            if (String.IsNullOrEmpty(varValue))
                throw new ArgumentNullException(varName, "参数不合法，不应为空或空字符串");
        }

        /// <summary>
        /// 校验数字
        /// </summary>
        /// <param name="varValue"></param>
        /// <param name="varName"></param>
        public static void IsNumberOfVar(this string varValue, string varName)
        {
            if (!Regex.IsMatch(varValue, "^[0-9]*$"))
                throw new ArgumentNullException(varName, "参数不合法，不是数字串");
        }

        public static T GetInstance<T>() where T : class
        {
            object obj = Activator.CreateInstance(typeof(T), true);
            return (T)obj;
        }

        public static byte[] StreamToBytes(this Stream stream)

        {

            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 

            stream.Seek(0, SeekOrigin.Begin);

            return bytes;

        }

        public static string GetEmailTemplateContent(this TemplateModel templateModel, string templatePath)
        {
            if (!File.Exists(templatePath))
            {
                throw new MissingRequiredFieldException("文件不存在，路径：" + templatePath);
            }
            string htmlContent = File.ReadAllText(templatePath);
            PropertyInfo[] pro = (typeof(TemplateModel)).GetProperties();
            foreach (var item in pro)
            {
                object value = templateModel.GetType().GetProperty(item.Name).GetValue(templateModel, null);
                if (value != null)
                {
                    htmlContent = htmlContent.Replace("${" + item.Name + "}", value.ToString());
                }
            }
            return htmlContent;
        }


        /// <summary>
        /// 获取内部错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string InnerException(this Exception ex)
        {
            if (ex.InnerException == null)
                return ex.Message;
            else
                return ex.InnerException.InnerException();
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOfVar(this string s)
        {
            return String.IsNullOrEmpty(s);
        }


        public static long ToLong(this string obj)
        {
            long id = 0;
            long.TryParse(obj, out id);
            return id;
        }

        public static long[] ToLongs(this string str)
        {
            try
            {
                string[] sArray = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return Array.ConvertAll(sArray, long.Parse);
            }
            catch
            {
                return new long[] { 0 };
            }
        }

        public static bool ToBool(this string str)
        {
            try
            {
                bool result = false;
                bool.TryParse(str, out result);
                return result;
            }
            catch
            {
                return false;
            }
        }


        public static string GetCurrentHttpUrl(this HttpRequest request)
        {
            string url = "http://" + request.Url.Host;

            if (request.Url.Port != 80)
            {
                url += ":" + request.Url.Port;
            }
            return url;

        }

        public static long GetFileLength(this string filePath)
        {
            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                long len = fileStream.Length;
                fileStream.Close();
                return len;
            }
            catch
            {
                return 0;
            }
        }

        public static string GetSizeDisplayAs(this long fileLength)
        {
            double size = 0;
            if (fileLength < 1024) // Byte
            {
                return GetSizeDisplay(size, "{0}B");
            }
            else
            {
                size = (float)(fileLength) / 1024;
                if (size < 1024) // KB
                {
                    return GetSizeDisplay(size, "{0}KB");
                }
                else
                {
                    size = size / 1024;
                    if (size < 1024) // MB
                    {
                        return GetSizeDisplay(size, "{0}MB");
                    }
                    else
                    {
                        size = size / 1024;
                        if (size < 1024) // GB
                        {
                            return GetSizeDisplay(size, "{0}GB");
                        }
                        else
                        {
                            size = size / 1024; //TB
                            return GetSizeDisplay(size, "{0}TB");
                        }
                    }
                }
            }
        }

        private static string GetSizeDisplay(double size, string format)
        {
            int integer = (int)(Math.Floor(size));
            if (integer == size)
            {
                return string.Format(format, integer);
            }
            else
            {
                return string.Format(format, Math.Round(size, 2).ToString());
            }
        }



    }


    public class PinYin
    {
        private static string ConvertToABC(string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "j";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";
            return "*";
        }
        public static string GetABC(string chinese)
        {
            if (string.IsNullOrEmpty(chinese))
                return null;
            string ret = string.Empty;
            foreach (char c in chinese)
            {
                if (String.IsNullOrEmpty(c.ToString().Trim()))
                    continue;
                if ((int)c >= 33 && (int)c <= 126)
                {
                    ret += c.ToString().ToUpper();
                }
                else
                {
                    ret += ConvertToABC(c.ToString()).ToUpper();
                }
            }
            return ret;
        }
    }
}
