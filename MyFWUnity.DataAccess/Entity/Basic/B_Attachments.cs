using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.DataAccess.Entity
{
    [DB(DbName = DbName.MyFWUnityBasicDB)]
    public partial class B_Attachments
    {
        public string ID { get; set; }
        public string WebUrl { get; set; }
        public string Name { get; set; }

        public string FilePath { get; set; }

        public string Remark { get; set; }

        public string EntityId { get; set; }

        public string EntityType { get; set; }

        public DateTime CreateDate { get; set; }

        public string SizeDisplay { get; set; }
        public long Size { get; set; }
    }
}
