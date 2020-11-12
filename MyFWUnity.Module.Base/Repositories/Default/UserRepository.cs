using MyFWUnity.Core.Repositories;
using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Module.Base.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.Repositories.Default
{
    public class UserRepository : EFRepository<B_User>, IUserRepository
    {
        public UserRepository()
            : base()
        {
          
        }
    }
}
