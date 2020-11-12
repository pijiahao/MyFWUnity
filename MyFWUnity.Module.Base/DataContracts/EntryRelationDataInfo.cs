using MyFWUnity.Common;
using MyFWUnity.Core.Model;
using MyFWUnity.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.DataContracts
{
    public enum EntryType
    {
        Entry,
        Permission,
    }
    public enum RelationType
    {
        User,
        Project
    }

    public class EntryRelationDataInfo : BaseDataModel<EntryRelationDataInfo, B_EntryRelation>
    {
        public string EntryID { get; set; }
        public string EntryType { get; set; }
        public string ProjectID { get; set; }
        public string RelationID { get; set; }
        public string RelationType { get; set; }
    }

}
