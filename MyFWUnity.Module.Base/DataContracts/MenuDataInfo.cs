using MyFWUnity.Common;
using MyFWUnity.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.DataContracts
{
    public class MenuDataInfo
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public string BindPermissionCode { get; set; }
        public bool IsShow { get; set; }
        public List<MenuDataInfo> Childrens { get; set; }
    }
}
