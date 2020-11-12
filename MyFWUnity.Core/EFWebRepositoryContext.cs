using MyFWUnity.Core.RepositoryContext;
using MyFWUnity.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Collections.Concurrent;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using MyFWUnity.Common.Module;
using MyFWUnity.Common;
using System.Data.Common;
using System.Data;
using System.Reflection;
using MyFWUnity.DataAccess;
using MySql.Data.MySqlClient;

namespace MyFWUnity.Core
{

    public class EFWebRepositoryContext : EFRepositoryContext
    {
        public static ConcurrentDictionary<Enum, string> _CONNCONFIG = new ConcurrentDictionary<Enum, string>();
        public static ConcurrentDictionary<Enum, DbContext> _DbContext = new ConcurrentDictionary<Enum, DbContext>();
        private static object LockObj = new object();
        public EFWebRepositoryContext()
        {
        }
        protected override DbContext GetContext(Enum database)
        {
            lock (LockObj)
            {
                if (!HasContext(database))
                {
                    InitializeContext(database);
                }
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[database.ToString()] as System.Data.Entity.DbContext;
                }
                else
                {
                    return _DbContext[database];
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            // HttpContext.Current.Items.Clear();
            GC.SuppressFinalize(this);
        }

        private void InitializeContext(Enum database)
        {
            var dbKind = ToDbKind(database);
            DbContext dbContext = null;
            if (!HasContext(database))
            {
                if (dbKind == DbKind.MySql)
                    dbContext = new DbContextEx(new MySqlConnection(Init(database)), true);
                if (dbKind == DbKind.Oracle)
                    dbContext = new DbContextEx(new EntityConnection(Init(database)), true);
                if (dbKind == DbKind.SqlServer)
                    dbContext = new DbContextEx(new SqlConnection(Init(database)), true);
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[database.ToString()] = dbContext;
                }
                else
                {
                    _DbContext.AddOrUpdate(database, dbContext, (key, value) => value);
                }

            }
        }

        public string Init(Enum dbName)
        {
            if (_CONNCONFIG.Where(w => w.Key == dbName).Count() == 0)
            {
                var x = ConfigurationManager.ConnectionStrings[dbName.ToString()].ConnectionString;
                _CONNCONFIG.AddOrUpdate(dbName, x, (key, value) => value);
                return x;
            }
            else
                return _CONNCONFIG.Where(w => w.Key == dbName).FirstOrDefault().Value;
        }

        public DbKind ToDbKind(Enum dbName)
        {
            var c = ConfigurationManager.ConnectionStrings[dbName.ToString()];
            if (c == null)
                throw new Exception(dbName.ToString() + "配置找不到.");
            var x = c.ProviderName;
            return x.Contains(DbKind.MySql.ToString()) ? DbKind.MySql : x.Contains(DbKind.Oracle.ToString()) ? DbKind.Oracle : DbKind.SqlServer;
        }

        private bool HasContext(Enum database)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Items.Contains(database.ToString()) && HttpContext.Current.Items[database.ToString()] != null;
            }
            else
            {
                return _DbContext.ContainsKey(database);

            }
        }
    }


}
