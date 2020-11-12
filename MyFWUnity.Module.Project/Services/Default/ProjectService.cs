using MyFWUnity.Common;
using MyFWUnity.Core.Repositories;
using MyFWUnity.Core.Services;

using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.Module.Project.DataContracts;
using MyFWUnity.Module.Project.Services.Interfaces;
using MyFWUnity.Module.Project.Repositories.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Core.Model;
using MyFWUnity.DataAccess;

namespace MyFWUnity.Module.Project.Services.Default
{
    public class ProjectService : BaseService, IProjectService
    {
        public ProjectService() : base()
        {
          
        }
        [Dependency]
        public IProjectRepository ProjectRepository { get; set; }

        [Dependency]
        public IEntryRelationService EntryRelationService { get; set; }
        [Dependency]
        public IUserService UserService { get; set; }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<ProjectDataInfo> QueryPage(string condition, int pageIndex, int pageSize, out long recordCount)
        {
            ProjectRepository.Initialize(DbName.MyFWUnityExtendDB);
            IList<P_Project> list = null;
            Expression<Func<P_Project, bool>> expression = null;
            if (!string.IsNullOrEmpty(condition))
            {
                expression = n => n.Name.Contains(condition);
            }
            list = ProjectRepository.LoadPageList(out recordCount, pageIndex, pageSize, expression, n => n.ID, false);
            if (list == null)
            {
                return null;
            }
            return ToolsEx.GetInstance<ProjectDataInfo>().ToConvertForMember(list, (c) =>
            {
                foreach (var item in c)
                {
                    UserDataInfo user = UserService.GetUserByID(item.CreateUserID);
                    item.CreateUser = new Dictionary<string, string>();
                    item.CreateUser.Add(user.ID, user.Name);
                    string entityType = new P_Project().GetType().Name;
                    List<B_Attachments> b_Attachments = AttachmentsRepository.FindList(n => n.EntityId == item.ID && n.EntityType == entityType).ToList();
                    if (b_Attachments != null)
                    {
                        item.Attachments = ToolsEx.GetInstance<AttachmentsDataInfo>().ToConvertForMember(b_Attachments);
                    }
                }
                return c;
            });
        }

        public List<ProjectDataInfo> GetProjectByUserID(string userID)
        {
            string[] projectIDs = EntryRelationService.GetEntryRelationProjectByUserID(EntryType.Permission.ToString(), RelationType.User.ToString(), userID);
            IList<P_Project> list = ProjectRepository.FindList(n => projectIDs.Contains(n.ID));
            return ToolsEx.GetInstance<ProjectDataInfo>().ToConvertForMember(list);
        }


        public List<ProjectDataInfo> All()
        {
            IList<P_Project> list = ProjectRepository.FindList();
            return ToolsEx.GetInstance<ProjectDataInfo>().ToConvertForMember(list);
        }
    }
}
