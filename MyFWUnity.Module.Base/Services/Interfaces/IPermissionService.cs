using MyFWUnity.Core.Services;
using MyFWUnity.Module.Base.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.Services.Interfaces
{
    public interface IPermissionService
    {
        void AddRelationByPermission(string permissionID, string userID, string projectID = "");
        void AddRelationByPermissions(string[] permissionIDs, string userID, string projectID = "");
        List<PermissionClass> QueryAllPermissionClass();
        List<PermissionDataInfo> QueryAllPermissionByUserID(string userID,string projectID="", bool isAdministrator = false);

        List<PermissionClass> QueryAllPermissionClassByUserID(string userID, string projectID = "");
    }
}
