using MyFWUnity.Core.Services;
using MyFWUnity.Module.Base.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.Services.Interfaces
{
    public interface IMenuService
    {
        List<MenuDataInfo> GetMenuListData(List<string> permissionData);
    }
}
