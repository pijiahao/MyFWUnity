using MyFWUnity.Common;
using MyFWUnity.Core.Services;

using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
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

namespace MyFWUnity.WebApp.WebAPI.APIController.Base
{
    public class PermissionController : BaseApiController
    {
        [Dependency]
        public IPermissionService PermissionService { get; set; }

        [Dependency]
        public IEntryRelationService EntryRelationService { get; set; }

        [HttpGet]
        public HttpResponseMessage AddRelationByPermission(string permissionID, string userID, string projectID)
        {
            return ReturnResult(() =>
            {
                PermissionService.AddRelationByPermission(permissionID, userID, projectID);
                return ResultJson.BuildEmptySuccessJsonResponse();
            });
        }

        [HttpGet]
        public HttpResponseMessage AddRelationByPermissions(string permissionIDs, string userID, string projectID)
        {
            return ReturnResult(() =>
            {
                PermissionService.AddRelationByPermissions(permissionIDs.Split(','), userID, projectID);
                return ResultJson.BuildEmptySuccessJsonResponse();
            });
        }

        [HttpGet]
        public HttpResponseMessage QueryAllPermissionClass(string userID, string projectID)
        {
            return ReturnResult(() =>
            {
                List<PermissionClass> permissionClasses = PermissionService.QueryAllPermissionClassByUserID(userID, projectID);
                return ResultJson.BuildJsonResponse(permissionClasses, Infrastructure.Model.ResultData.MessageType.None, null);
            });
        }


        [HttpGet]
        public HttpResponseMessage QueryAllPermission(string userID, string projectID)
        {
            return ReturnResult(() =>
            {
                List<PermissionDataInfo> permissionDataInfos = PermissionService.QueryAllPermissionByUserID(userID, projectID);
                return ResultJson.BuildJsonResponse(permissionDataInfos, Infrastructure.Model.ResultData.MessageType.None, null);
            });
        }

    }
}
