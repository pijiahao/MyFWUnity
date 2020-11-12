using MyFWUnity.Common;
using MyFWUnity.Common.Module;
using MyFWUnity.Common.Services;
using MyFWUnity.Core.Infrastructure.DatabaseContext;
using MyFWUnity.Core.Model;
using MyFWUnity.Core.Repositories;
using MyFWUnity.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Repositories
{
    public class EFRepository<TEntity> where TEntity : class
    {
        private IEFRepositoryContext mobjContext = null;

        public IRepositoryContext Context
        {
            get { return mobjContext; }
        }

        public void Initialize(DbName database = DbName.MyFWUnityBasicDB)
        {
            Context.Initialize(database);
        }

        private void ReLoadDB()
        {
            var dbName = typeof(TEntity).GetCustomAttributeValue<DBAttribute, DbName>(x => x.DbName);
            Initialize(dbName);
        }

        public EFRepository()
        {
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            if (context is IEFRepositoryContext)
            {
                mobjContext = context as IEFRepositoryContext;
            }
        }

        public void Add(TEntity entity)
        {
            ReLoadDB();
            mobjContext.Add<TEntity>(entity);
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            ReLoadDB();
            foreach (TEntity entity in entities)
            {
                Add(entity);
            }
        }

        public void Update(TEntity entity)
        {
            ReLoadDB();
            mobjContext.Update<TEntity>(entity);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            ReLoadDB();
            foreach (TEntity entity in entities)
            {
                Update(entity);
            }
        }

        public void DeleteByKey(params object[] keyValues)
        {
            ReLoadDB();
            TEntity defaultEntity = this.FindByKeyValues(keyValues);
            if (defaultEntity != null)
                mobjContext.Delete<TEntity>(defaultEntity);
        }

        public void Delete(TEntity entity)
        {
            ReLoadDB();
            mobjContext.Delete<TEntity>(entity);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            ReLoadDB();
            foreach (TEntity entity in entities)
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="includePath">to get related data at one time, EF use latency loading as default</param>
        /// <returns></returns>
        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression = null, params string[] includePath)
        {
            ReLoadDB();
            IQueryable<TEntity> defaultQuery = mobjContext.Context.Set<TEntity>();
            if (includePath != null)
            {
                foreach (string path in includePath)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        defaultQuery = defaultQuery.Include(path);
                    }
                }
            }

            if (expression != null)
                defaultQuery = defaultQuery.Where(expression);
            return defaultQuery;
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression = null, params string[] includePath)
        {
            IQueryable<TEntity> defaultQuery = Query(expression, includePath);
            return defaultQuery.FirstOrDefault();
        }

        public TEntity FindByKeyValues(params object[] keyValues)
        {
            ReLoadDB();
            return mobjContext.Context.Set<TEntity>().Find(keyValues);
        }

        public IList<TEntity> FindList(Expression<Func<TEntity, bool>> expression = null, params string[] includePath)
        {
            IQueryable<TEntity> defaultQuery = Query(expression, includePath);
            return defaultQuery.ToList();
        }
        public IList<TEntity> FindDistinctList(Expression<Func<TEntity, bool>> expression = null, params string[] includePath)
        {
            IQueryable<TEntity> defaultQuery = Query(expression, includePath);
            return defaultQuery.Distinct().ToList();
        }

        public IList<TEntity> FindListByOrder<TKey>(Expression<Func<TEntity, bool>> expression = null, Expression<Func<TEntity, TKey>> orderBy = null, bool ascending = true, params string[] includePath)
        {
            IQueryable<TEntity> defaultQuery = Query(expression, includePath);
            if (orderBy != null)
            {
                if (ascending)
                    defaultQuery = defaultQuery.OrderBy(orderBy);
                else
                    defaultQuery = defaultQuery.OrderByDescending(orderBy);
            }

            return defaultQuery.ToList();
        }

        public IList<TEntity> LoadPageList<TKey>(out long count, int pageIndex, int pageSize, Expression<Func<TEntity, bool>> expression = null, Expression<Func<TEntity, TKey>> orderBy = null, bool ascending = true, params string[] includePath)
        {
            IQueryable<TEntity> defaultQuery = Query(expression, includePath);
            if (orderBy != null)
            {
                if (ascending)
                    defaultQuery = defaultQuery.OrderBy(orderBy);
                else
                    defaultQuery = defaultQuery.OrderByDescending(orderBy);
            }
            count = defaultQuery.Count();
            defaultQuery = defaultQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return defaultQuery.ToList();
        }

        public IList<TEntity> SqlQueryList(string sqlQueryScript, params object[] parameters)
        {
            ReLoadDB();
            return mobjContext.Context.Set<TEntity>().SqlQuery(sqlQueryScript, parameters).ToList();
        }

        public IEnumerable<object> QueryEntities(string sqlQueryScript, params object[] parameters)
        {
            LogModule.Debug(string.Format("Query entity by sql {0}", sqlQueryScript));
            return SqlQueryList(sqlQueryScript, parameters);
        }
    }
}
