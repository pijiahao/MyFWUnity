using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common.Integration.Interfaces
{
    public interface ILoginInfoPersistenceService
    {
        string CurrentUserID { get; }
        void SaveLoginUser(string userID);
        void DeleteUser();
    }
   
}
