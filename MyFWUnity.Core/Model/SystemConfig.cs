
using MyFWUnity.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Model
{

    public class SystemConfig : BaseDataModel<SystemConfig, B_SystemConfig>
    {
        public string PlatformName { get; set; }
        public string PlatformLogo { get; set; }
        public string EmailTemplate { get; set; }
        public string LogModuleInfo { get; set; }

        [ForMember]
        public Dictionary<string, string> LogModule { get; set; }
    }

}
