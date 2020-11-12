using MyFWUnity.Common.Integration.Interfaces;
using MyFWUnity.Core.Model;
using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.Module.Project.DataContracts;
using MyFWUnity.Module.Project.Services.Interfaces;
using MyFWUnity.WebApp.Infrastructure.Filters;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyFWUnity.WebApp.Infrastructure
{
    [AuthorizeFilter]
    [ViewBagFilter]
    public abstract class BaseController : Controller
    {
        [Dependency]
        public ILoginInfoPersistenceService LoginInfoPersistenceService { get; set; }

        [Dependency]
        public IMenuService MenuService { get; set; }

        [Dependency]
        public IPermissionService PermissionService { get; set; }

        [Dependency]
        public IUserService UserService { get; set; }

        [Dependency]
        public IProjectService ProjectService { get; set; }

        [Dependency]
        public ISysService SysService { get; set; }

        public string CurrentUser
        {
            get
            {
                return LoginInfoPersistenceService.CurrentUserID;
            }
        }
        public SystemConfig SystemConfig
        {
            get
            {
                return SysService.GetSystemConfig();
            }
        }


        private UserDataInfo _user = null;
        public UserDataInfo LoginUser
        {
            get
            {
                if (_user == null)
                {
                    _user = UserService.GetUserByID(CurrentUser);

                }
                return _user;
            }
        }
        private List<ProjectDataInfo> _userProject = null;
        public List<ProjectDataInfo> UserProject
        {
            get
            {
                if (_userProject == null)
                {
                    _userProject = ProjectService.GetProjectByUserID(CurrentUser);

                }
                return _userProject;
            }
        }

        public virtual void UpdateViewBag()
        {
            List<string> permissionData = GetPermissionData();
            ViewBag.MenuData = MenuService.GetMenuListData(permissionData);
            ViewBag.PermissionClassData = PermissionService.QueryAllPermissionClass();
            ViewBag.UserInfo = LoginUser;
        }

        private List<string> GetPermissionData()
        {
            List<string> permissionData = new List<string>();
            List<PermissionDataInfo> permissionDataInfos = PermissionService.QueryAllPermissionByUserID(CurrentUser, "", LoginUser.IsAdmin);
            if (permissionDataInfos != null)
            {
                permissionData = permissionDataInfos.Where(n => n.IsChecked).Select(n => n.Code).ToList();
            }
            return permissionData;
        }
    }
}
