using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Core.Repositories.System.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Repositories.System.Default
{
    public class SystemConfigRepository : EFRepository<B_SystemConfig>, ISystemConfigRepository
    {
        public SystemConfigRepository()
            : base()
        {
           
        }
    }
}
