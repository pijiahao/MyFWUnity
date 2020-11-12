using MyFWUnity.Core.Services;
using MyFWUnity.Module.Project.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Project.Services.Interfaces
{
    public interface IProjectService : IBaseService
    {
        List<ProjectDataInfo> QueryPage(string condition, int pageIndex, int pageSize, out long recordCount);
        List<ProjectDataInfo> All();
        List<ProjectDataInfo> GetProjectByUserID(string userID);
    }
}
