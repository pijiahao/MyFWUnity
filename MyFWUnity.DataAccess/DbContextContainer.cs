using MyFWUnity.Common;
using MyFWUnity.Common.Module;
using MyFWUnity.DataAccess.Entity;
using Microsoft.Practices.Unity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.DataAccess
{
    public enum DbKind
    {
        MySql,
        Oracle,
        SqlServer
    }
    public enum DbName
    {
        MyFWUnityBasicDB,
        MyFWUnityExtendDB
    }
    public class DbContextContainer
    {
        public static ConcurrentDictionary<Enum, string> _CONNCONFIG = new ConcurrentDictionary<Enum, string>();
        public DbContextEx _DataAccess { get; set; }
        public DbContextContainer(DbKind dbKind, DbName dbName)
        {
            var builder = new UnityContainer();
            //builder.RegisterInstance(o => BuildInstance(dbKind, dbName));
            //_DataAccess = builder.Build().Resolve<DbContextEx>();
            //_DataAccess.OpenConnection();
            //_DataAccess._DbKind = dbKind;
        }
        public DbContextContainer(DbName dbName)
        {
            var dbKind = ToDbKind(dbName);
            var builder = new UnityContainer();
            //builder.Register(o => BuildInstance(dbKind, dbName));
            //_DataAccess = builder.Build().Resolve<DbContextEx>();
            //_DataAccess.OpenConnection();
            //_DataAccess._DbKind = dbKind;
        }
        public DbContextEx BuildInstance(DbKind dbKind, DbName dbName)
        {
            if ("IsDesDataLink".ConfigValue("NO") == "YES")
            {
                if (dbKind == DbKind.MySql)
                    return new DbContextEx(new MySqlConnection(Init(dbName)), true);
                if (dbKind == DbKind.Oracle)
                    return new DbContextEx(new EntityConnection(Init(dbName)), true);
                if (dbKind == DbKind.SqlServer)
                    return new DbContextEx(new SqlConnection(Init(dbName)), true);
            }
            return new DbContextEx(dbName.ToString());
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
        public DbKind ToDbKind(DbName dbName)
        {
            var c = ConfigurationManager.ConnectionStrings[dbName.ToString()];
            if (c == null)
                throw new Exception(dbName.ToString() + "配置找不到.");
            var x = c.ProviderName;
            return x.Contains(DbKind.MySql.ToString()) ? DbKind.MySql : x.Contains(DbKind.Oracle.ToString()) ? DbKind.Oracle : DbKind.SqlServer;
        }
        public void Dispose()
        {
            if (_DataAccess != null)
                _DataAccess.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    public class DbContextEx : DbContext
    {
        public string _DBKey => "DBKey".ConfigValue();
        public DbKind _DbKind { get; set; }
        public DbConnection DbConnection { get; set; }
        public DbContextEx(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.SetInitializer<DbContextEx>(null);
            DbConnection = Database.Connection;
        }
        public DbContextEx(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Database.SetInitializer<DbContextEx>(null);
            DbConnection = Database.Connection;
        }

        public string DbConfigValue(string vName, string vInit = "")
        {
            return DbConfigValue(w => w.DBKEY == _DBKey && w.PARAMNAME == vName, vInit);
        }
        public string DbConfigValue(Func<Db_Params, bool> _Func, string vInit = "")
        {
            var dbParams = Set<Db_Params>().AsNoTracking().Where(_Func).FirstOrDefault();
            return dbParams == default(Db_Params) ? vInit :
                               (dbParams.PARAMVAL.IsNullOrEmptyOfVar() ? dbParams.DEFAULTVAL : dbParams.PARAMVAL);
        }
        public string HCDbConfigValue(string vName, string vInit = "")
        {
            return HCDbConfigValue(w => w.DB_KEY == _DBKey && w.PARAM_NAME == vName, vInit);
        }
        public string HCDbConfigValue(Func<Db_Param, bool> _Func, string vInit = "")
        {
            var dbParams = Set<Db_Param>().AsNoTracking().Where(_Func).FirstOrDefault();
            return dbParams == default(Db_Param) ? vInit :
                               (dbParams.PARAM_VAL.IsNullOrEmptyOfVar() ? dbParams.DEFAULT_VAL : dbParams.PARAM_VAL);
        }
        public DbConnection OpenConnection()
        {
            if (Database.Connection.State == ConnectionState.Closed)
                Database.Connection.Open();
            return Database.Connection;
        }
        /// <summary>
        /// 初始化 OnModelCreating 
        /// </summary>
        public void Initializer()
        {
            OnModelCreating(new DbModelBuilder());
        }

        /// <summary>
        /// 18-05-28 add gpw
        /// 返回具体错误信息。
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException exception)
            {
                var errorMessages =
                    exception.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);

                var fullErrorMessage = string.Join(", ", errorMessages);
                var exceptionMessage = string.Concat(exception.Message, " 验证异常消息是：", fullErrorMessage);
                //记录日志
                LogModule.Error(exceptionMessage);
                throw new DbEntityValidationException(exceptionMessage, exception.EntityValidationErrors);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException());
            }
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="dbType">参数类型</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数传输类型</param>
        /// <param name="size">获取或设置列中的数据的最大大小（以字节为单位）</param>
        /// <returns></returns>
        public DbParameter CreateParameter(string paraName, DbType dbType, object value, ParameterDirection direction = ParameterDirection.Input, int size = 0)
        {
            var cmd = Database.Connection.CreateCommand();
            dynamic command = cmd;
            command.BindByName = true;
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = paraName;
            parameter.DbType = dbType;
            parameter.Direction = direction;
            parameter.Value = value;
            parameter.Size = size;
            return parameter;
        }
        /// <summary>
        /// 执行存储过程不带事务
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DbCommand ExeProcedure(string procedureName, params object[] parameters)
        {
            return ExeProcedure(procedureName, null, parameters);
        }
        /// <summary>
        /// 执行存储过程带事务
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="tran">事务</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DbCommand ExeProcedure(string procedureName, DbTransaction tran = null, params object[] parameters)
        {
            try
            {
                var cmd = Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procedureName;
                if (tran != null)
                    cmd.Transaction = tran;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                int resule = cmd.ExecuteNonQuery();
                return cmd;
            }
            catch (Exception ex)
            {
                if (tran != null)
                    tran.Rollback();
                throw ex;
            }
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //  throw new UnintentionalCodeFirstException();
            //modelBuilder.HasDefaultSchema("");
            //modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
            //base.OnModelCreating(modelBuilder);
        }
        protected override void Dispose(bool disposing)
        {
            if (Database.Connection.State == ConnectionState.Open)
                Database.Connection.Close();
            base.Dispose(disposing);
        }
        public virtual DbSet<B_Attachments> B_Attachments { get; set; }
        public virtual DbSet<B_EmailMessage> B_EmailMessage { get; set; }
        public virtual DbSet<B_EntryRelation> B_EntryRelation { get; set; }
        public virtual DbSet<B_Log> B_Log { get; set; }
        public virtual DbSet<B_User> B_User { get; set; }
        public virtual DbSet<P_Project> P_Project { get; set; }
        // public virtual DbSet<B_SystemConfig> B_SystemConfig { get; set; }
    }
}
