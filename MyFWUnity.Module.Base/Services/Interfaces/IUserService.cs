using MyFWUnity.Core.Model.DataContracts;
using MyFWUnity.Core.Services;
using MyFWUnity.Module.Base.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.Services.Interfaces
{
    public interface IUserService : IBaseService
    {
        void CreateAdminUser();
        UserDataInfo GetUserByID(string userID);
        UserDataInfo GetUserDataInfoByLogin(string userCode, string password);
        string[] GetUserIDsByCondition(string condition);
        List<UserDataInfo> QueryPage(string condition, int pageIndex, int pageSize, out long recordCount);
       
    }
}
