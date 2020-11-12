using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityRepositoryAttribute : Attribute
    {
        public string EntityClassName { get; set; }

        public EntityRepositoryAttribute()
        {
            EntityClassName = string.Empty;
        }
    }
}
