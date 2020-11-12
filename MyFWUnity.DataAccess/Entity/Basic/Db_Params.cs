using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.DataAccess.Entity
{
    public class Db_Params
    {
        public int Id { get; set; }
        public string DBKEY { get; set; }
        public string CLASSID { get; set; }
        public string PARAMNAME { get; set; }
        public string PARAMDES { get; set; }
        public string PARAMVAL { get; set; }
        public string DEFAULTVAL { get; set; }
        public string PARAMVALDES { get; set; }
    }
    public class Db_ParamsMapper : EntityTypeConfiguration<Db_Params>
    {
        public Db_ParamsMapper()
        {
            ToTable("UT_Params");
            HasKey(o => o.Id);
        }
    }
}
