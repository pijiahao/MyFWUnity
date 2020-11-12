using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Infrastructure.DatabaseContext
{
    public interface IEFRepositoryContext : IRepositoryContext
    {

        DbContext Context { get; }
    }
}
