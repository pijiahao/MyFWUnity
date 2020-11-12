using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.DataAccess.Entity
{
    [DB(DbName = DbName.MyFWUnityBasicDB)]
    public class B_Message
    {
        public long Id { get; set; }
        public string ReceivedUserUniqueId { get; set; }
        public string SenderUserUniqueId { get; set; }
        public string Body { get; set; }
        public int IsReceived { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Type { get; set; }
        public string EntityUniqueId { get; set; }
    }
}
