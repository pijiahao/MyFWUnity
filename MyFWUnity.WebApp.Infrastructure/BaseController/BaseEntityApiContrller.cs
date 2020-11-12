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
using System.Web;
using MyFWUnity.Common.Module;

namespace MyFWUnity.WebApp.Infrastructure
{

    [WebApiControllerExceptionFilter]
    public abstract class BaseEntityApiContrller<TSource, TEntity> : System.Web.Http.ApiController where TSource : BaseDataModel<TSource, TEntity>, new() where TEntity : class
    {
        public BaseEntityApiContrller()
        {
            CommonService = new CommonService<TSource, TEntity>();

        }
        public CommonService<TSource, TEntity> CommonService { get; set; }
        [Dependency]
        public IUserService UserService { get; set; }

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
                return CommonService.LoginInfoPersistenceService.CurrentUserID;
            }
        }

        public string WinServerUserID { get; set; }

        public SystemConfig SystemConfig
        {
            get
            {
                return CommonService.SystemConfig;
            }
        }

        /// <summary>
        /// 单个修改或者新增
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResponseMessage AddOrUpdateByFormData()
        {
            return ReturnResult(() =>
            {
                string message = string.Empty;
                TSource source = Bind<TSource>();
                source.OperatorIsAdministrator = LoginUser.IsAdmin;
                source.Attachments = UploadUtil.GetAttachmentsDataInfos();
                if (string.IsNullOrEmpty(source.ID))
                {
                    message = "添加成功";
                    CommonService.Add(source);
                }
                else
                {
                    message = "修改成功";
                    CommonService.Update(source);
                }

                return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.Information, message);
            });
        }

        /// <summary>
        /// 单个修改或者新增
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResponseMessage AddOrUpdate(TSource source)
        {
            return ReturnResult(() =>
            {
                source.WinServerUserID = WinServerUserID;
                string message = string.Empty;
                source.OperatorIsAdministrator = LoginUser.IsAdmin;
                source.Attachments = UploadUtil.GetAttachmentsDataInfos();
                if (string.IsNullOrEmpty(source.ID))
                {
                    message = "添加成功";
                    CommonService.Add(source);
                }
                else
                {
                    message = "修改成功";
                    CommonService.Update(source);
                }

                return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.Information, message);
            });
        }
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResponseMessage Add(List<TSource> sources)
        {
            return ReturnResult(() =>
            {
                sources.ForEach(x =>
                {
                    x.WinServerUserID = WinServerUserID;
                    x.OperatorIsAdministrator = LoginUser.IsAdmin;
                });
                CommonService.Add(sources);
                return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.Information, "批量添加成功");
            });
        }
        /// <summary>
        /// 新增和删除
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResponseMessage AddAndDelete()
        {
            return ReturnResult(() =>
            {
                string sourceStr = HttpContext.Current.Request.Form["source"];
                string idStr = HttpContext.Current.Request.Form["id"];
                bool isAlert = HttpContext.Current.Request.Form["isAlert"].ToBool();
                TSource source = null;
                long id = 0;
                if (!string.IsNullOrEmpty(sourceStr) && sourceStr != "{}")
                {
                    source = sourceStr.J2Entity<TSource>();
                    source.OperatorIsAdministrator = LoginUser.IsAdmin;
                    CommonService.Add(source);
                }
                if (!string.IsNullOrEmpty(idStr))
                {
                    id = idStr.ToLong();
                    CommonService.Delete(id);
                }
                if (isAlert)
                {
                    return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.Information, "批量添加和删除成功");
                }
                else
                {
                    return ResultJson.BuildEmptySuccessJsonResponse();
                }
            });
        }

        /// <summary>
        /// 批量新增和批量修改
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResponseMessage AddsAndDeletes()
        {
            return ReturnResult(() =>
            {
                string list = HttpContext.Current.Request.Form["list"];
                string ids = HttpContext.Current.Request.Form["ids"];
                List<TSource> sources = new List<TSource>();
                long[] idlist = new long[] { 0 };
                if (!string.IsNullOrEmpty(list))
                {
                    sources = list.J2Entity<List<TSource>>();
                    sources.ForEach(x =>
                    {
                        x.OperatorIsAdministrator = LoginUser.IsAdmin;
                    });
                    CommonService.Add(sources);
                }
                if (!string.IsNullOrEmpty(ids))
                {
                    idlist = ids.ToLongs();
                    CommonService.Delete(idlist);
                }
                return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.Information, "批量添加和删除成功");
            });
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResponseMessage Update(List<TSource> sources)
        {
            return ReturnResult(() =>
            {
                sources.ForEach(x =>
                {
                    x.WinServerUserID = WinServerUserID;
                    x.OperatorIsAdministrator = LoginUser.IsAdmin;
                });
                CommonService.Update(sources);
                return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.Information, "批量修改成功");
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual HttpResponseMessage Remove(long id)
        {
            return ReturnResult(() =>
            {
                CommonService.Delete(id);
                return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.Information, "删除成功");
            });
        }
        /// <summary>
        ///批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual HttpResponseMessage Remove(long[] ids)
        {
            return ReturnResult(() =>
            {
                CommonService.Delete(ids);
                return ResultJson.BuildJsonResponse(null, Infrastructure.Model.ResultData.MessageType.Information, "批量删除成功");
            });
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
                        CommonService.WinServerUserID = token.FirstOrDefault();
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

        public virtual T Bind<T>() where T : class, new()
        {
            var Request = System.Web.HttpContext.Current.Request;
            if (Request.HttpMethod == "GET")
            {
                var req = HttpUtility.UrlDecode(Request.Url.Query.Replace("?", ""));
                var entity = new T();
                if (req.IsNullOrEmptyOfVar())
                    return entity;
                var kv = new Dictionary<String, String>();
                req.Split('&').ToList().ForEach(o =>
                {
                    if (o.Contains('='))
                        kv.Add(o.Split('=')[0].ToUpper(), o.Split('=')[1]);
                });
                entity.GetType().GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(NotMapper))).ToList().ForEach(o =>
                {
                    o.SetValue(entity, (kv.Where(w => w.Key == o.Name.ToUpper()).Count() > 0 ? kv[o.Name.ToUpper()] : null), null);
                });
                return entity;

            }
            else if (Request.HttpMethod == "POST")
            {
                var entity = new T();
                entity.GetType().GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(NotMapper))).ToList().ForEach(o =>
                {
                    o.SetValue(entity, Request.Form[o.Name]);
                });
                return entity;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 非映射 用于Model对象的Bind
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NotMapper : Attribute
    {

    }
}
