using MyFWUnity.Core.Repositories;
using MyFWUnity.DataAccess;
using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Module.Project.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Project.Repositories.Default
{
    public class ProjectRepository : EFRepository<P_Project>, IProjectRepository
    {
        public ProjectRepository()
            : base()
        {
         
        }

    }
}
