using MyFWUnity.Core.Model;
using MyFWUnity.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Infrastructure.DatabaseContext
{
    public interface IRepositoryContext : ITransactionProcessor, IDisposable
    {
        void Initialize(DbName database=DbName.MyFWUnityBasicDB);
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Save();

    }
}
