using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Model
{
    /// <summary>
    /// automapper formember
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ForMemberAttribute : Attribute
    {
        public ForMemberAttribute()
        {
        }
    }
}
