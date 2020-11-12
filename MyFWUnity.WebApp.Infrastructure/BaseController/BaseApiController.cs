using MyFWUnity.Common;
using MyFWUnity.Common.Services;
using MyFWUnity.Core.Services;
using MyFWUnity.Core.Model;
using MyFWUnity.WebApp.Infrastructure.Utilities;
using MyFWUnity.WebApp.Infrastructure.Utilities.ExceptionHandler;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MyFWUnity.Module.Base.DataContracts;

using MyFWUnity.Common.Integration.Interfaces;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.Common.Module;

namespace MyFWUnity.WebApp.Infrastructure
{
    [WebApiControllerExceptionFilter]
    public abstract class BaseApiController : System.Web.Http.ApiController
    {
        public BaseApiController()
        {
        }

        [Dependency]
        public ILoginInfoPersistenceService LoginInfoPersistenceService { get; set; }

        [Dependency]
        public ISysService SysService { get; set; }
        [Dependency]
        public IUserService UserService { get; set; }
        public string WinServerUserID { get; set; }
        private UserDataInfo _user = null;
        public UserDataInfo LoginUser
        {
            get
            {
                if (_user == null)
                {
                    if (string.IsNullOrEmpty(CurrentUserID))
                    {
                        _user = UserService.GetUserByID(WinServerUserID);
                    }
                    else
                    {
                        _user = UserService.GetUserByID(CurrentUserID);
                    }


                }
                return _user;
            }
        }

        public string CurrentUserID
        {
            get
            {
                return LoginInfoPersistenceService.CurrentUserID;
            }
        }
        public SystemConfig SystemConfig
        {
            get
            {
                return SysService.GetSystemConfig();
            }
        }


        public virtual HttpResponseMessage ReturnResult(Func<HttpResponseMessage> fuc)
        {
            LogModule.Info("Start Api:" + Request.RequestUri.ToString());
            bool isVerify = false;
            if (!string.IsNullOrEmpty(CurrentUserID))
            {
                isVerify = true;
            }
            if (Request.Headers.Contains("UserID"))
            {
                List<string> token = Request.Headers.GetValues("UserID").ToList();
                if (token != null)
                {
                    if (!string.IsNullOrEmpty(token.FirstOrDefault()))
                    {

                        WinServerUserID = token.FirstOrDefault();
                        if (LoginUser != null)
                        {
                            isVerify = true;
                        }
                    }
                }
            }
            if (!isVerify)
            {
                return ResultJson.FailtoJson("用户未登录，调用api失败", System.Net.HttpStatusCode.Unauthorized);
            }
            HttpResponseMessage httpResponseMessage = fuc();
            LogModule.Info("End Api");
            return httpResponseMessage;
        }
    }
}
