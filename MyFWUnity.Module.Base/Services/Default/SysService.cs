using MyFWUnity.Common;
using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Core.Model;
using MyFWUnity.Core.Model.DataContracts;
using MyFWUnity.Core.Services;

using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.Services.Default
{
    public class SysService : BaseService, ISysService
    {

        [Dependency]
        public IUserService UserService { get; set; }
        public SystemConfig GetSystemConfig()
        {
            return SystemConfig;
        }

        public void SetSystemConfig(SystemConfig systemConfig)
        {
            B_SystemConfig sysConfig = SystemConfigRepository.FirstOrDefault();
            if (sysConfig != null)
            {
                sysConfig = systemConfig.UpdateEntity(systemConfig, sysConfig);
                SystemConfigRepository.Update(sysConfig);
            }
            else
            {
                sysConfig = systemConfig.CreateNew(systemConfig);
                SystemConfigRepository.Add(sysConfig);
            }
            this.Context.Save();
        }

        /// <summary>
        /// 日志分页查询数据
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<LogDataInfo> LogQueryPage(string condition, int pageIndex, int pageSize, out long recordCount)
        {
            IList<B_Log> list = null;
            Expression<Func<B_Log, bool>> expression = null;
            if (!string.IsNullOrEmpty(condition))
            {
                string[] userIDs = UserService.GetUserIDsByCondition(condition);
                expression = n => n.ModulePage.Contains(condition) || n.Description.Contains(condition) || n.NewData.Contains(condition) || n.OldData.Contains(condition) || userIDs.Contains(n.UserID);
            }
            list = LogRepository.LoadPageList(out recordCount, pageIndex, pageSize, expression, n => n.CreateDate, false);
            if (list == null)
            {
                return null;
            }
            return ToolsEx.GetInstance<LogDataInfo>().ToConvertForMember(list, (o) =>
            {
                foreach (var item in o)
                {
                    UserDataInfo user = UserService.GetUserByID(item.UserID);
                    if (user != null)
                    {
                        if (!string.IsNullOrEmpty(item.OldData))
                        {
                            item.Description = string.Format(item.Description, user.Name, item.OldData, item.NewData);
                        }
                        else if (!string.IsNullOrEmpty(item.NewData))
                        {
                            item.Description = string.Format(item.Description, user.Name, item.NewData);
                        }
                        else
                        {
                            item.Description = string.Format(item.Description, user.Name);
                        }
                        item.CreateUser = new Dictionary<string, string>();
                        item.CreateUser.Add(user.ID, user.Name);
                    }

                }
                return o;
            });
        }

        public void DeleteAttachmentsById(string id)
        {
            B_Attachments b_Attachments = AttachmentsRepository.FindByKeyValues(id);
            if (b_Attachments != null)
            {
                AttachmentsRepository.Delete(b_Attachments);
            }
            this.Context.Save();
        }


        public AttachmentsDataInfo GetAttachmentsById(string id)
        {
            B_Attachments b_Attachments = AttachmentsRepository.FindByKeyValues(id);
            return ToolsEx.GetInstance<AttachmentsDataInfo>().MapTo(b_Attachments);
        }
    }
}
