using MyFWUnity.Common;
using MyFWUnity.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.DataContracts
{
    public class PermissionClass
    {
        public string Name { get; set; }
        public bool IsSystemShow { get; set; }
        public bool IsProjectShow { get; set; }
        public List<PermissionDataInfo> PermissionDataInfos { get; set; }
    }

    public class PermissionDataInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsChecked { get; set; }
        public string EntryRelationID { get; set; }

    }
}
