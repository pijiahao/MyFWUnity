using MyFWUnity.Common.Integration.Interfaces;
using MyFWUnity.Common.Services;
using MyFWUnity.Core.Model;
using MyFWUnity.DataAccess;
using MyFWUnity.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Project.DataContracts
{

    public class ProjectDataInfo : BaseDataModel<ProjectDataInfo, P_Project>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string CreateUserID { get; set; }
        [ForMember]
        public Dictionary<string, string> CreateUser { get; set; }

        protected override void StoreUnchangedFieldsForAdd()
        {
            this.ID = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            ILoginInfoPersistenceService LoginInfoPersistenceService = ServiceLocator.Instance.GetService(typeof(ILoginInfoPersistenceService)) as ILoginInfoPersistenceService;
            if (string.IsNullOrEmpty(LoginInfoPersistenceService.CurrentUserID))
            {
                this.CreateUserID = this.WinServerUserID;
            }
            else
            {
                this.CreateUserID = LoginInfoPersistenceService.CurrentUserID;
            }
        }

        protected override void StoreUnchangedFieldsForUpdate(P_Project entity)
        {
            this.CreateDate = entity.CreateDate;
            this.CreateUserID = entity.CreateUserID;
        }
    }
}
