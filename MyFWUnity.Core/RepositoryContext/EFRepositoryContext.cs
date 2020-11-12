using MyFWUnity.Common.Module;
using MyFWUnity.Core.Infrastructure.DatabaseContext;
using MyFWUnity.Core.Model;
using MyFWUnity.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.RepositoryContext
{
    public abstract class EFRepositoryContext : IEFRepositoryContext
    {
        protected abstract System.Data.Entity.DbContext GetContext(Enum database);

        private Enum mEFDatabase = null;

        public Enum MyEFDatabase
        {
            get { return mEFDatabase; }
            set { mEFDatabase = value; }
        }


        public System.Data.Entity.DbContext Context
        {
            get
            {
                return GetContext(MyEFDatabase);
            }
        }

        public virtual void Initialize(DbName database=DbName.MyFWUnityBasicDB)
        {
            MyEFDatabase = database;
            GetContext(database);
        }

        public virtual void Add<T>(T entity) where T : class
        {
            if (Context != null)
            {
                Context.Set<T>().Add(entity);
            }
            else
            {
                LogModule.Warn("Missing DB Context");
            }
        }
        private bool HandleDetached<T>(T entity) where T : class
        {
            var objectContext = ((IObjectContextAdapter)Context).ObjectContext;
            var entitySet = objectContext.CreateObjectSet<T>();
            var entityKey = objectContext.CreateEntityKey(entitySet.EntitySet.Name, entity);
            object foundSet;
            bool exists = objectContext.TryGetObjectByKey(entityKey, out foundSet);
            if (exists)
            {
                objectContext.Detach(foundSet); //从上下文中移除
            }
            return exists;
        }
        public virtual void Update<T>(T entity) where T : class
        {
            if (Context != null)
            {
                if (Context.Entry<T>(entity).State == EntityState.Detached)
                {
                    HandleDetached(entity);
                }
                Context.Set<T>().Attach(entity);
                Context.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                LogModule.Warn("Missing DB Context");
            }
        }

        public virtual void Delete<T>(T entity) where T : class
        {
            if (Context != null)
            {
                Context.Set<T>().Remove(entity);
            }
            else
            {
                LogModule.Warn("Missing DB Context");
            }
        }

        public virtual void Save()
        {
            if (Context != null)
            {
                Context.SaveChanges();
            }
            else
            {
                LogModule.Warn("Missing DB Context");
            }
        }

        public virtual void BeginTransaction()
        {
            if (Context != null && Context.Database == null)
            {
                LogModule.Debug("Begin Transaction");
                Context.Database.BeginTransaction();
                LogModule.Debug("Transaction started");
            }
        }

        public virtual void Commit()
        {
            if (Context != null && Context.Database.CurrentTransaction != null)
            {
                LogModule.Debug("Start to Commit");
                Context.Database.CurrentTransaction.Commit();
                LogModule.Debug("Committed");
            }
        }

        public virtual void Rollback()
        {
            if (Context != null && Context.Database.CurrentTransaction != null)
            {
                LogModule.Debug("Start to rollback");
                Context.Database.CurrentTransaction.Rollback();
                LogModule.Debug("Rollback");
            }
        }


        public virtual void Dispose()
        {
            try
            {
                if (Context != null)
                {
                    if (Context.Database.CurrentTransaction != null)
                    {
                        Context.Database.CurrentTransaction.Dispose();
                    }

                    if (Context.Database.Connection.State != System.Data.ConnectionState.Closed)
                    {
                        Context.Database.Connection.Close();
                    }
                    Context.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogModule.Error("Faile to dispose DB context", ex);
            }
        }
    }
}
