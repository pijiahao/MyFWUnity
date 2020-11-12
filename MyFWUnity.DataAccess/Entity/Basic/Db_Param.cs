using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.DataAccess.Entity
{
    public class Db_Param
    {
        public Int32 PARAM_ID { get; set; }
        public String PARAM_NAME { get; set; }
        public String PARAM_DES { get; set; }
        public String PARAM_VAL_DES { get; set; }
        public String DEFAULT_VAL { get; set; }
        public String QUICK_CODE1 { get; set; }
        public String QUICK_CODE2 { get; set; }
        public String MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public String CLASS_ID { get; set; }
        public Int32? ENTITY_FLAG { get; set; }
        public String ENTITY_SQL { get; set; }
        public String QUICK_CODE3 { get; set; }
        public String DB_KEY { get; set; }
        public String PARAM_VAL { get; set; }
    }
    public class Db_ParamMapper : EntityTypeConfiguration<Db_Param>
    {
        public Db_ParamMapper()
        {
            ToTable("CT_Param");
            HasKey(k => k.PARAM_ID);
        }
    }
}
