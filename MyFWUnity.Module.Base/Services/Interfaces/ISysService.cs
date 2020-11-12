using MyFWUnity.Core.Model;
using MyFWUnity.Core.Model.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.Services.Interfaces
{
    public interface ISysService
    {
        SystemConfig GetSystemConfig();
        void SetSystemConfig(SystemConfig systemConfig);
        List<LogDataInfo> LogQueryPage(string condition, int pageIndex, int pageSize, out long recordCount);
        void DeleteAttachmentsById(string id);

        AttachmentsDataInfo GetAttachmentsById(string id);
    }
}
