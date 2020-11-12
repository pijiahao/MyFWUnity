using MyFWUnity.Common.Encrypt;
using MyFWUnity.Common.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common.Cookie
{
    public static class CookieUtil
    {
        public static void CreateCookie(object obj, string CookieName)
        {
            try
            {
                RemoveCookie(CookieName);
                DateTime NowDate = DateTime.Now;
                System.Web.HttpCookie IMCookie = new System.Web.HttpCookie(CookieName);//初使化并设置Cookie的名称        
                IMCookie.Values.Add("obj", EncryptManager.Encode(obj.ToString()));//   
                IMCookie.Values.Add("e", EncryptManager.Encode(NowDate.AddHours(1).ToString()));//存储到期时间
                System.Web.HttpContext.Current.Response.AppendCookie(IMCookie);                                                      //IMCookie.Values.Add("obj", Json);//
                                                                                                                                     // IMCookie.Values.Add("e", NowDate.AddHours(1).ToString());//存储到期时间

            }
            catch (Exception ex)
            {
                LogModule.Error(ex.Message);
            }
        }

        public static string GetCookie(string CookieName)
        {
            string result = "";
            try
            {
                if (System.Web.HttpContext.Current.Request.Cookies[CookieName] != null)
                {
                    System.Web.HttpCookie IMCookie = System.Web.HttpContext.Current.Request.Cookies[CookieName];
                    DateTime endDate = DateTime.Parse(EncryptManager.Decode(IMCookie["e"]));
                    // DateTime endDate = DateTime.Parse(IMCookie["e"]);
                    if (endDate > DateTime.Now)
                    {
                        RemoveCookie(CookieName);
                        System.Web.HttpCookie NewDiyCookie = new System.Web.HttpCookie(CookieName);
                        NewDiyCookie.Values.Add("obj", IMCookie["obj"]);
                        NewDiyCookie.Values.Add("e", EncryptManager.Encode(DateTime.Now.AddHours(1).ToString()));//存储到期时间
                        System.Web.HttpContext.Current.Response.AppendCookie(NewDiyCookie);
                        result = EncryptManager.Decode(IMCookie["obj"]);
                        //  result = IMCookie["obj"];
                    }
                }
            }
            catch (Exception ex)
            {
                LogModule.Error(ex.Message);
            }
            return result;
        }

        public static void DelCookie(string CookieName)
        {
            try
            {
                if (System.Web.HttpContext.Current.Request.Cookies[CookieName] != null)
                {
                    System.Web.HttpCookie IMCookie = System.Web.HttpContext.Current.Request.Cookies[CookieName];
                    DateTime endDate = DateTime.Parse(EncryptManager.Decode(IMCookie["e"]));
                    RemoveCookie(CookieName);
                    System.Web.HttpCookie NewDiyCookie = new System.Web.HttpCookie(CookieName);
                    NewDiyCookie.Values.Add("obj", IMCookie["obj"]);
                    NewDiyCookie.Values.Add("e", EncryptManager.Encode(DateTime.Now.AddHours(-1).ToString()));//存储到期时间
                    System.Web.HttpContext.Current.Response.AppendCookie(NewDiyCookie);

                }
            }
            catch (Exception ex)
            {
                LogModule.Error(ex.Message);
            }
        }

        private static void RemoveCookie(string CookieName)
        {
            for (int i = 0; i < System.Web.HttpContext.Current.Request.Cookies.Count; i++)
            {
                if (System.Web.HttpContext.Current.Request.Cookies[CookieName] != null)
                {
                    System.Web.HttpContext.Current.Request.Cookies.Remove(CookieName);
                }
            }
        }
    }
}
