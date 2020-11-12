using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Infrastructure.DatabaseContext
{
    public interface ITransactionProcessor
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
