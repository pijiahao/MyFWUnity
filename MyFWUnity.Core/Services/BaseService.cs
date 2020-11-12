using MyFWUnity.Common;
using MyFWUnity.Common.Services;
using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Core.Infrastructure.DatabaseContext;
using MyFWUnity.Core.Model;
using MyFWUnity.Core.Model.DataContracts;
using MyFWUnity.Core.Repositories;
using MyFWUnity.Core.Repositories.System.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFWUnity.DataAccess;

namespace MyFWUnity.Core.Services
{
    public abstract class BaseService : IBaseService
    {
        private readonly IEFRepositoryContext mobjContext = null;
        private SystemConfig _systemConfig = null;
        public ISystemConfigRepository SystemConfigRepository { get; set; }
        public ILogRepository LogRepository { get; set; }

        protected IMessageRepository MessageRepository { get; set; }

        protected IAttachmentsRepository AttachmentsRepository { get; set; }
        public BaseService()
        {
            LogRepository = ServiceLocator.Instance.GetService<ILogRepository>();
            SystemConfigRepository = ServiceLocator.Instance.GetService<ISystemConfigRepository>();
            AttachmentsRepository = ServiceLocator.Instance.GetService<IAttachmentsRepository>();
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            if (context is IEFRepositoryContext)
            {
                this.mobjContext = context as IEFRepositoryContext;
            }
            if (this.Context != null)
            {
                mobjContext.Initialize();
            }
        }


        public System.Data.Entity.DbContext DB
        {
            get
            {
                return Context.Context;
            }
        }

        public SystemConfig SystemConfig
        {
            get
            {
                if (_systemConfig != null)
                {
                    return _systemConfig;
                }
                else
                {
                    SystemConfig systemConfig = null;
                    B_SystemConfig sysConfig = SystemConfigRepository.FirstOrDefault();
                    if (sysConfig != null)
                    {
                        systemConfig = ToolsEx.GetInstance<SystemConfig>().MapTo(sysConfig);
                        systemConfig.LogModule = systemConfig.LogModuleInfo.J2Entity<Dictionary<string, string>>();
                        _systemConfig = systemConfig;
                    }
                    return systemConfig;
                }
            }
        }

        protected IEFRepositoryContext Context
        {
            get { return this.mobjContext; }
        }

        public virtual void LogAdd(LogDataInfo logDataInfo, string modifier)
        {
            logDataInfo.ID = Guid.NewGuid().ToString();
            logDataInfo.CreateDate = DateTime.Now;
            logDataInfo.UserID = modifier;
            B_Log log = logDataInfo.CreateNew(logDataInfo);
            LogRepository.Add(log);
            this.Context.Save();
        }

        public virtual void MessageAdd(MessageDataInfo messageDataInfo)
        {
            messageDataInfo.CreateDate = DateTime.Now;
            messageDataInfo.IsReceived = 0;
            B_Message message = messageDataInfo.CreateNew(messageDataInfo);
            MessageRepository.Add(message);
            this.Context.Save();
        }

        public virtual void AttachmentsAdd(AttachmentsDataInfo attachmentsDataInfo)
        {
            attachmentsDataInfo.ID = Guid.NewGuid().ToString();
            attachmentsDataInfo.CreateDate = DateTime.Now;
            attachmentsDataInfo.Size = attachmentsDataInfo.FilePath.GetFileLength();
            attachmentsDataInfo.SizeDisplay = attachmentsDataInfo.Size.GetSizeDisplayAs();
            B_Attachments attachments = attachmentsDataInfo.CreateNew(attachmentsDataInfo);
            AttachmentsRepository.Add(attachments);
            this.Context.Save();
        }
        public virtual void MessageDeleteByTypeAndEntityId(string type, string entityUniqueId)
        {
            IList<B_Message> messages = MessageRepository.FindList(n => n.Type == type && n.EntityUniqueId == entityUniqueId);
            if (messages != null)
            {
                foreach (var item in messages)
                {
                    MessageRepository.Delete(item);
                }
            }
            this.Context.Save();
        }
    }
}
