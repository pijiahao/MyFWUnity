using MyFWUnity.Common;
using MyFWUnity.Common.Services;
using MyFWUnity.Core.Repositories;
using MyFWUnity.Core.Model;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyFWUnity.Core.Model.DataContracts;

using MyFWUnity.Common.Module;
using MyFWUnity.Common.Integration.Interfaces;
using MyFWUnity.DataAccess;

namespace MyFWUnity.Core.Services
{

    public enum CommonServiceType
    {
        Add,
        Update,
        Delete
    }
    public class CommonService<TSource, TEntity> : BaseService where TEntity : class where TSource : BaseDataModel<TSource, TEntity>
    {
        public CommonService()
        {
            Repository = new EFRepository<TEntity>();
            LoginInfoPersistenceService = ServiceLocator.Instance.GetService(typeof(ILoginInfoPersistenceService)) as ILoginInfoPersistenceService;

        }

        public string WinServerUserID { get; set; }
        protected EFRepository<TEntity> Repository { get; set; }

        public ILoginInfoPersistenceService LoginInfoPersistenceService { get; set; }

        #region BacicData

        private void BacicData(TSource source, TEntity entity, CommonServiceType commonServiceType)
        {
            if (commonServiceType != CommonServiceType.Delete)//删除不需要附加数据
            {
                Message(source, entity);
                Attachments(source, entity);
            }
            switch (commonServiceType)
            {
                case CommonServiceType.Add:
                    LogAdd(new LogDataInfo()
                    {
                        Description = "{0}新增了一条数据，数据源：{1}",
                        ModulePage = entity.GetType().Name,
                        NewData = source.ToJson(),
                    });
                    break;
                case CommonServiceType.Update:
                    LogAdd(new LogDataInfo()
                    {
                        Description = "{0}修改了一条数据，修改前数据：{1}，修改后数据：{2}",
                        ModulePage = entity.GetType().Name,
                        NewData = source.ToJson(),
                        OldData = entity.ToJson()
                    });
                    break;
                case CommonServiceType.Delete:
                    LogAdd(new LogDataInfo()
                    {
                        Description = "{0}删除了一条数据，数据源：{1}",
                        ModulePage = entity.GetType().Name,
                        OldData = entity.ToJson()
                    });
                    break;
            }
        }

        private void Message(TSource source, TEntity entity)
        {
            List<MessageDataInfo> message = null;
            if (source.Message != null)
            {
                message = source.Message;
            }
            if (!string.IsNullOrEmpty(source.MessageStr))
            {
                if (message == null)
                {
                    message = new List<MessageDataInfo>();
                }
                message.AddRange(source.MessageStr.J2Entity<List<MessageDataInfo>>());
            }
            if (message != null)
            {
                message.ForEach(x =>
                {
                    x.Type = entity.GetType().Name;
                    MessageAdd(x);
                });
            }

        }

        private void Attachments(TSource source, TEntity entity)
        {
            List<AttachmentsDataInfo> attachmentsDataInfos = null;
            if (source.Attachments != null)
            {
                attachmentsDataInfos = source.Attachments;
            }
            if (!string.IsNullOrEmpty(source.AttachmentsStr))
            {
                if (attachmentsDataInfos == null)
                {
                    attachmentsDataInfos = new List<AttachmentsDataInfo>();
                }
                attachmentsDataInfos.AddRange(source.AttachmentsStr.J2Entity<List<AttachmentsDataInfo>>());
            }
            if (attachmentsDataInfos != null)
            {
                attachmentsDataInfos.ForEach(x =>
                {
                    x.EntityType = entity.GetType().Name;
                    x.EntityId = source.ID;
                    AttachmentsAdd(x);
                });
            }

        }

        public void LogAdd(LogDataInfo logDataInfo)
        {
            if (string.IsNullOrEmpty(LoginInfoPersistenceService.CurrentUserID))
            {
                LogAdd(logDataInfo, WinServerUserID);
            }
            else
            {
                LogAdd(logDataInfo, LoginInfoPersistenceService.CurrentUserID);
            }

        }

        #endregion

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="source"></param>
        public virtual void Add(TSource source)
        {
            source.CreateDate = DateTime.Now;
            TEntity entity = source.CreateNew(source);
            Repository.Add(entity);
            LogModule.Debug(string.Format("{0}==========>新增了一条数据，数据源{1}", entity.GetType().Namespace, source.ToJson()));
            this.Context.Save();
            BacicData(source, entity, CommonServiceType.Add);

        }
        /// <summary>
        ///  批量新增
        /// </summary>
        /// <param name="source"></param>
        public virtual void Add(List<TSource> sources)
        {
            foreach (var source in sources)
            {
                source.CreateDate = DateTime.Now;
                TEntity entity = source.CreateNew(source);
                Repository.Add(entity);
                this.Context.Save();
                BacicData(source, entity, CommonServiceType.Add);
                LogModule.Debug(string.Format("{0}==========>新增了一条数据，数据源{1}", entity.GetType().Namespace, source.ToJson()));
            }


        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="source"></param>
        public virtual void Update(TSource source)
        {
            TEntity entity = Repository.FindByKeyValues(source.ID);
            if (entity != null)
            {
                entity = source.UpdateEntity(source, entity);
                Repository.Update(entity);
                this.Context.Save();
                BacicData(source, entity, CommonServiceType.Update);
                LogModule.Debug(string.Format("{0}==========>修改了一条数据，修改前数据：{1}，修改后数据：{2}", entity.GetType().Namespace, entity.ToJson(), source.ToJson()));
            }

        }

        /// <summary>
        ///  批量修改
        /// </summary>
        /// <param name="source"></param>
        public virtual void Update(List<TSource> sources)
        {
            foreach (var source in sources)
            {
                TEntity entity = Repository.FindByKeyValues(source.ID);
                if (entity != null)
                {
                    entity = source.UpdateEntity(source, entity);
                    Repository.Update(entity);
                    this.Context.Save();
                    BacicData(source, entity, CommonServiceType.Update);
                    LogModule.Debug(string.Format("{0}==========>修改了一条数据，修改前数据：{1}，修改后数据：{2}", entity.GetType().Namespace, entity.ToJson(), source.ToJson()));

                }
            }

        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(long id)
        {
            TEntity entity = Repository.FindByKeyValues(id);
            if (entity != null)
            {
                Repository.Delete(entity);
                this.Context.Save();
                BacicData(null, entity, CommonServiceType.Delete);
                LogModule.Debug(string.Format("{0}==========>删除了一条数据，数据源：{1}", entity.GetType().Namespace, entity.ToJson()));

            }

        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(long[] ids)
        {
            foreach (var id in ids)
            {
                TEntity entity = Repository.FindByKeyValues(id);
                if (entity != null)
                {
                    Repository.Delete(entity);
                    this.Context.Save();
                    //  MessageDeleteByTypeAndEntityId(entity.GetType().Name, id);
                    BacicData(null, entity, CommonServiceType.Delete);
                    LogModule.Debug(string.Format("{0}==========>删除了一条数据，数据源：{1}", entity.GetType().Namespace, entity.ToJson()));
                }
            }

        }




    }
}
