
using MyFWUnity.Module.Project.DataContracts;
using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Module.Project.Services.Interfaces;
using MyFWUnity.WebApp.Infrastructure;
using MyFWUnity.WebApp.Infrastructure.Utilities;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MyFWUnity.Common.Services;

namespace MyFWUnity.WebApp.WebAPI.APIController.Project
{
    public class ProjectController : BaseEntityApiContrller<ProjectDataInfo, P_Project>
    {
        [Dependency]
        public IProjectService ProjectService { get; set; }
        [HttpGet]
        public HttpResponseMessage QueryPage(string condition, int pageIndex, int pageSize)
        {
            return ReturnResult(() =>
            {
                long recordCount = 0;
                List<ProjectDataInfo> projectDataInfos = ProjectService.QueryPage(condition, pageIndex, pageSize, out recordCount);
                return ResultJson.BuildJsonResponse(new { rows = projectDataInfos, total = recordCount }, Infrastructure.Model.ResultData.MessageType.None, null);

            });
        }

        [HttpGet]
        public HttpResponseMessage All()
        {
            return ReturnResult(() =>
            {
                List<ProjectDataInfo> projectDataInfos = ProjectService.All();
                return ResultJson.BuildJsonResponse(projectDataInfos, Infrastructure.Model.ResultData.MessageType.None, null);

            });
        }
    }
}
