using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Model.DataContracts
{

    public class LogDataInfo : BaseDataModel<LogDataInfo, B_Log>
    {
        public string ModulePage { get; set; }
        public string Description { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string UserID { get; set; }

        [ForMember]
        public Dictionary<string, string> CreateUser { get; set; }
    }
}
