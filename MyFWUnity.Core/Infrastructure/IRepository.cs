using MyFWUnity.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Initialize(DbName database =DbName.MyFWUnityBasicDB);

        /// <summary>
        /// Return IQueryable without actually query DB 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="includePath"></param>
        /// <returns></returns>
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression = null, params string[] includePath);

        /// <summary>
        /// Find the first object which mataches the expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression = null, params string[] includePath);

        /// <summary>
        /// Finds an entity with the given primary key values.
        /// The ordering of composite key values is as defined in the EDM
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity FindByKeyValues(params object[] keyValues);

        /// <summary>
        /// 根据指定条件表达式得到数据查询列表
        /// if no expression, resurn all
        /// </summary>
        IList<TEntity> FindList(Expression<Func<TEntity, bool>> expression = null, params string[] includePath);

        IList<TEntity> FindDistinctList(Expression<Func<TEntity, bool>> expression = null, params string[] includePath);

        IList<TEntity> FindListByOrder<TKey>(Expression<Func<TEntity, bool>> expression = null, Expression<Func<TEntity, TKey>> orderBy = null, bool ascending = true, params string[] includePath);

        /// <summary>
        /// Add entity into DB context
        /// </summary>
        /// <param name="entity"></param>
        void Add(TEntity entity);

        /// <summary>
        /// Add a collection of entities
        /// </summary>
        /// <param name="entities"></param>
        void Add(IEnumerable<TEntity> entities);

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Update(TEntity entity);

        /// <summary>
        /// Update a collection of entities
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Remove entity by key or keys
        /// </summary>
        /// <param name="keyValues"></param>
        void DeleteByKey(params object[] keyValues);

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// Remove a collection of entities
        /// </summary>
        /// <param name="entity"></param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// 分页获取全部集合
        /// </summary>
        /// <param name="count">返回的记录总数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>集合</returns>
        IList<TEntity> LoadPageList<TKey>(out long count, int pageIndex, int pageSize, Expression<Func<TEntity, bool>> expression = null, Expression<Func<TEntity, TKey>> orderBy = null, bool ascending = true, params string[] includePath);

        IList<TEntity> SqlQueryList(string sqlQueryScript, params object[] parameters);
    }
}
