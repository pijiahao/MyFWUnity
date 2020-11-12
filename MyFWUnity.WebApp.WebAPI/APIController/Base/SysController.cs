using MyFWUnity.Common;
using MyFWUnity.Common.Email;
using MyFWUnity.Core.Model.DataContracts;
using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.WebApp.Infrastructure;
using MyFWUnity.WebApp.Infrastructure.Model.ResultData;
using MyFWUnity.WebApp.Infrastructure.Utilities;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MyFWUnity.WebApp.WebAPI.APIController.Base
{
    public class SysController : BaseApiController
    {
        [Dependency]
        public IEmailMessageService EmailMessageService { get; set; }
        [Dependency]
        public ISysService SysService { get; set; }

        [HttpGet]
        public HttpResponseMessage LogQueryPage(string condition, int pageIndex, int pageSize)
        {
            return ReturnResult(() =>
            {
                long recordCount = 0;
                List<LogDataInfo> logDataInfos = SysService.LogQueryPage(condition, pageIndex, pageSize, out recordCount);
                return ResultJson.BuildJsonResponse(new { rows = logDataInfos, total = recordCount }, Infrastructure.Model.ResultData.MessageType.None, null);

            });
        }
        [HttpGet]
        public HttpResponseMessage EmailQueryPage(string condition, int pageIndex, int pageSize)
        {
            return ReturnResult(() =>
            {
                long recordCount = 0;
                List<EmailMessageDataInfo> emailMessageDataInfos = EmailMessageService.QueryPage(condition, pageIndex, pageSize, out recordCount);
                return ResultJson.BuildJsonResponse(new { rows = emailMessageDataInfos, total = recordCount }, Infrastructure.Model.ResultData.MessageType.None, null);

            });
        }

        [HttpGet]
        public HttpResponseMessage DeleteAttachmentsById(string id)
        {
            return ReturnResult(() =>
            {
                SysService.DeleteAttachmentsById(id);
                return ResultJson.BuildEmptySuccessJsonResponse();

            });
        }

        public HttpResponseMessage UploadImage()
        {
            return ReturnResult(() =>
            {
                string errorInfo = string.Empty;
                string webImageUrl = UploadUtil.UploadImage("/data/image/", ref errorInfo);
                if (string.IsNullOrEmpty(webImageUrl))
                {
                    return ResultJson.BuildJsonResponse(null, MessageType.Warning, errorInfo);
                }
                return ResultJson.BuildJsonResponse(webImageUrl, MessageType.None, null);
            });
        }

        [HttpGet]
        public HttpResponseMessage SendEmail()
        {
            return ReturnResult(() =>
            {
                TemplateModel templateModel = new TemplateModel();
                templateModel.Title = "看AV吗？";
                templateModel.TitleHerf = "http://www.baidu.com";
                templateModel.CloseStatus = "不看AV";
                templateModel.CurrentStatus = "看AV";
                templateModel.Date = DateTime.Now.ToString("yyyy-MM-dd");
                templateModel.Number = "www.看AV";
                templateModel.NumberHerf = "http://www.baidu.com";
                templateModel.Operation = "转换";
                templateModel.UserFace = "https://avatar.csdnimg.cn/8/6/7/1_qq_17486399.jpg";
                templateModel.UserHref = "http://www.baidu.com";
                templateModel.UserName = "张狗狗看AV";
                templateModel.PlatformName = "AV";
                string templatePath = HttpContext.Current.Server.MapPath("~/Template/Email/Template.html");
                string body = templateModel.GetEmailTemplateContent(templatePath);
                EmailMessageService.SendMsg(new EmailMessageDataInfo()
                {
                    Body = body,
                    IsBodyHTML = true,
                    Subject = "张狗狗",
                    SendTo = "2847803082@qq.com,443115515@qq.com,923704090@qq.com",
                    ReceiveUsers = "4832b746-797b-4595-87b5-96cc071d9dca",
                    From = ""
                });
                return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.None, null);

            });
        }
    }
}
