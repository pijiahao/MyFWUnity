using MyFWUnity.Core.Repositories.System.Interfaces;
using MyFWUnity.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Repositories.System.Default
{
    public class AttachmentsRepository : EFRepository<B_Attachments>, IAttachmentsRepository
    {
        public AttachmentsRepository()
            : base()
        {
         
        }
    }
}
