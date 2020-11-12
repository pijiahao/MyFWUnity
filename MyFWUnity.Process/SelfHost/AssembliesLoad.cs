using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

namespace MyFWUnity.Process
{
    public class AssembliesLoad : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public AssemblyElementCollection AssemblyNames
        {
            get { return (AssemblyElementCollection)this[""]; }
        }

        public static AssembliesLoad GetSection()
        {
            return ConfigurationManager.GetSection("AssembliesLoad") as AssembliesLoad;
        }
    }
    public class AssemblyElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            AssemblyElement serviceTypeElement = (AssemblyElement)element;
            return serviceTypeElement.AssemblyName;
        }
    }

    public class AssemblyElement : ConfigurationElement
    {
        [ConfigurationProperty("assemblyName", IsRequired = true)]
        public string AssemblyName
        {
            get { return (string)this["assemblyName"]; }
            set { this["assemblyName"] = value; }
        }
    }


    public class WebApiResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            AssembliesLoad settings = AssembliesLoad.GetSection();
            if (null != settings)
            {
                foreach (AssemblyElement element in settings.AssemblyNames)
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(element.AssemblyName);
                    if (!AppDomain.CurrentDomain.GetAssemblies().Any(assembly =>
                        AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), assemblyName)))
                    {
                        AppDomain.CurrentDomain.Load(assemblyName);
                    }
                }
            }
            return base.GetAssemblies();
        }
    }
}
