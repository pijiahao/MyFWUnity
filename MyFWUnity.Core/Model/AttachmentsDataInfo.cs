using MyFWUnity.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Model
{
    public class AttachmentsDataInfo : BaseDataModel<AttachmentsDataInfo, B_Attachments>
    {
        public string Name { get; set; }
        public string WebUrl { get; set; }
        public string FilePath { get; set; }

        public string Remark { get; set; }

        public string EntityId { get; set; }

        public string EntityType { get; set; }

        public string SizeDisplay { get; set; }
        public long Size { get; set; }
    }
}
