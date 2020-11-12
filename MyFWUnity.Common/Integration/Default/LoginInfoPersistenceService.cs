using MyFWUnity.Common.Cookie;
using MyFWUnity.Common.Integration.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common.Integration.Default
{
    public class LoginInfoPersistenceService : ILoginInfoPersistenceService
    {
        public static ConcurrentDictionary<string, string> _DbContext = new ConcurrentDictionary<string, string>();

        public string CurrentUserID
        {
            get
            {
                return CookieUtil.GetCookie(CookieName.IMCurrentUser);
            }
        }
        public void SaveLoginUser(string userID)
        {
            CookieUtil.CreateCookie(userID, CookieName.IMCurrentUser);
        }

        public void DeleteUser()
        {
            CookieUtil.DelCookie(CookieName.IMCurrentUser);
        }
    }
}
