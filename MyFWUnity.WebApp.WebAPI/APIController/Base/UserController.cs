using MyFWUnity.Common;
using MyFWUnity.Common.Email;
using MyFWUnity.Common.Encrypt;
using MyFWUnity.Core.Model.DataContracts;
using MyFWUnity.Core.Services;

using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.WebApp.Infrastructure;
using MyFWUnity.WebApp.Infrastructure.Model.ResultData;
using MyFWUnity.WebApp.Infrastructure.Utilities;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MyFWUnity.WebApp.WebAPI.APIController.Base
{
    public class UserController : BaseEntityApiContrller<UserDataInfo, B_User>
    {
        /// <summary>
        /// 去除底层验证
        /// </summary>
        /// <param name="fuc"></param>
        /// <returns></returns>
        public override HttpResponseMessage ReturnResult(Func<HttpResponseMessage> fuc)
        {
            return fuc();
        }
        [Dependency]
        public IUserService UserService { get; set; }



        [HttpGet]
        public HttpResponseMessage GetUserInfoByID(string id)
        {
            return ReturnResult(() =>
            {
                UserDataInfo userDataInfo = UserService.GetUserByID(id);
                return ResultJson.BuildJsonResponse(userDataInfo, Infrastructure.Model.ResultData.MessageType.None, null);

            });
        }


        [HttpGet]
        public HttpResponseMessage QueryPage(string condition, int pageIndex, int pageSize)
        {
            return ReturnResult(() =>
            {
                long recordCount = 0;
                List<UserDataInfo> userDataInfos = UserService.QueryPage(condition, pageIndex, pageSize, out recordCount);
                return ResultJson.BuildJsonResponse(new { rows = userDataInfos, total = recordCount }, Infrastructure.Model.ResultData.MessageType.None, null);

            });
        }




        public HttpResponseMessage UploadHeaderImage()
        {
            return ReturnResult(() =>
            {
                string errorInfo = string.Empty;
                string webImageUrl = UploadUtil.UploadImage("/data/image/", ref errorInfo);
                if (!string.IsNullOrEmpty(webImageUrl))
                {
                    this.CommonService.Update(new UserDataInfo()
                    {
                        ID = CurrentUserID,
                        UserFace = webImageUrl
                    });
                }
                else
                {
                    return ResultJson.BuildJsonResponse(null, MessageType.Warning, errorInfo);
                }
                return ResultJson.BuildJsonResponse(null, MessageType.None, null);
            });
        }

    }
}
