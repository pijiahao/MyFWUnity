using MyFWUnity.Common;
using MyFWUnity.Core.Infrastructure.Email;
using MyFWUnity.Core.Model;
using MyFWUnity.Core.Repositories;
using MyFWUnity.Core.Services;

using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.Module.Base.Repositories.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyFWUnity.DataAccess.Entity;

namespace MyFWUnity.Module.Base.Services.Default
{
    public class EmailMessageService : BaseService, IEmailMessageService
    {
        public EmailMessageService() : base()
        {
            EmailMessageCommonService = new CommonService<EmailMessageDataInfo, B_EmailMessage>();
        }

        [Dependency]
        public IEmailMessageRepository Repository { get; set; }

        [Dependency]
        public IEmailService EmailService { get; set; }

        [Dependency]
        public IUserService UserService { get; set; }

        public CommonService<EmailMessageDataInfo, B_EmailMessage> EmailMessageCommonService { get; set; }


        public void SendMsg(EmailMessageDataInfo emailMessageDataInfo)
        {
            if (EmailService.SendEmail(emailMessageDataInfo.MapTo<EmailMessage>()))
            {
                EmailMessageCommonService.Add(emailMessageDataInfo);
            }
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<EmailMessageDataInfo> QueryPage(string condition, int pageIndex, int pageSize, out long recordCount)
        {
            IList<B_EmailMessage> list = null;
            Expression<Func<B_EmailMessage, bool>> expression = null;
            if (!string.IsNullOrEmpty(condition))
            {
                expression = n => n.Subject.Contains(condition);
            }
            list = Repository.LoadPageList(out recordCount, pageIndex, pageSize, expression, n => n.ID, false);
            if (list == null)
            {
                return null;
            }
            return ToolsEx.GetInstance<EmailMessageDataInfo>().ToConvertForMember(list, (f) =>
            {
                foreach (var item in f)
                {
                    if (!string.IsNullOrEmpty(item.ReceiveUsers))
                    {
                        string[] receiveUserIDs = item.ReceiveUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        item.ReceiveUserNames = new Dictionary<string, string>();
                        foreach (var r in receiveUserIDs)
                        {
                            UserDataInfo user = UserService.GetUserByID(r);
                            if (user != null)
                            {
                                item.ReceiveUserNames.Add(r, user.Name);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(item.CCUsers))
                    {
                        string[] ccUserIDs = item.CCUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        item.CCUserNames = new Dictionary<string, string>();
                        foreach (var c in ccUserIDs)
                        {
                            UserDataInfo user = UserService.GetUserByID(c);
                            if (user != null)
                            {
                                item.CCUserNames.Add(c, user.Name);
                            }
                        }
                    }
                }
                return f;
            });
        }

    }
}
