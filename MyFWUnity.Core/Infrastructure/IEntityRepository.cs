using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Infrastructure
{
    public interface IEntityRepository
    {
        IEnumerable<object> QueryEntities(string sqlQueryScript, params object[] parameters);
    }
}
