using AutoMapper;
using MyFWUnity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Model
{
    public interface IDataModel<TSource, TEntity> where TEntity : class
    {
        TEntity CreateNew(TSource sourceData);
        TEntity UpdateEntity(TSource sourceData, TEntity existingEntity);
        TSource UpdateViewModel(TSource sourceData, TEntity entity);
    }

    public abstract class BaseDataModel<TSource, TEntity> : IDataModel<TSource, TEntity> where TEntity : class where TSource : class
    {
        public string WinServerUserID { get; set; }
        public string ID { get; set; }

        public System.DateTime CreateDate { get; set; }

        public List<MessageDataInfo> Message { get; set; }
        public string MessageStr { get; set; }

        public List<AttachmentsDataInfo> Attachments { get; set; }
        public string AttachmentsStr { get; set; }

        /// <summary>
        /// 是否管理员操作
        /// </summary>
        public bool OperatorIsAdministrator { get; set; }

        public Action<IMapperConfigurationExpression> GetMapperConfigurationExpression()
        {
            PropertyInfo[] pro = (typeof(TSource)).GetProperties();
            return (c =>
            {
                IMappingExpression<TEntity, TSource> mappingExpression = c.CreateMap<TEntity, TSource>();
                foreach (var item in pro)
                {
                    var attrName = item.GetCustomAttribute(typeof(ForMemberAttribute), true);
                    if (attrName != null)
                    {
                        mappingExpression.ForMember(item.Name, i => i.Ignore());
                    }
                }
            });
        }



        /// <summary>
        /// Convert current view model as a new entity
        /// </summary>
        /// <returns></returns>
        public virtual TEntity CreateNew(TSource sourceData)
        {
            StoreUnchangedFieldsForAdd();
            return sourceData.MapTo<TEntity>();
        }

        /// <summary>
        /// Update entity field values by current view model
        /// </summary>
        /// <param name="existingEntity"></param>
        /// <returns></returns>
        public TEntity UpdateEntity(TSource sourceData, TEntity existingEntity)
        {
            StoreUnchangedFieldsForUpdate(existingEntity);
            return UpdateExistingEntity(sourceData, existingEntity);
        }

        public virtual TSource UpdateViewModel(TSource sourceData, TEntity entity)
        {
            return entity.MapTo<TEntity, TSource>(sourceData);
        }
        public virtual TSource MapTo(TEntity entity)
        {
            TSource source = default(TSource);
            source = entity.MapTo<TEntity, TSource>(GetMapperConfigurationExpression());
            return source;
        }
        public virtual List<TSource> ToConvertForMember(IList<TEntity> entities)
        {
            IList<TSource> sources = default(List<TSource>);
            sources = entities.MapTo<TEntity, TSource>(GetMapperConfigurationExpression());
            return sources.ToList();
        }

        public virtual List<TSource> ToConvertForMember(IList<TEntity> entities, Func<IList<TSource>, IList<TSource>> func)
        {
            IList<TSource> sources = ToConvertForMember(entities);
            return func(sources).ToList();
        }




        protected virtual TEntity UpdateExistingEntity(TSource sourceData, TEntity existingEntity)
        {
            return sourceData.MapTo<TSource, TEntity>(existingEntity);
        }

        protected virtual void StoreUnchangedFieldsForUpdate(TEntity existingEntity)
        {
        }

        protected virtual void StoreUnchangedFieldsForAdd()
        {

        }

    }
}
