using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common.Config
{
    public class EmailServerSettingSection : ConfigurationSection
    {
        [ConfigurationProperty("SMTPHost", IsRequired = false, DefaultValue = "")]
        public string SMTPHost
        {
            get
            {
                return (string)base["SMTPHost"];
            }
            set
            {
                base["SMTPHost"] = value;
            }
        }

        [ConfigurationProperty("Port", IsRequired = false, DefaultValue = 25)]
        public int Port
        {
            get
            {
                return (int)base["Port"];
            }
            set
            {
                base["Port"] = value;
            }
        }

        [ConfigurationProperty("UserName", IsRequired = false, DefaultValue = "")]
        public string UserName
        {
            get
            {
                return (string)base["UserName"];
            }
            set
            {
                base["UserName"] = value;
            }
        }

        [ConfigurationProperty("Password", IsRequired = false, DefaultValue = "")]
        public string Password
        {
            get
            {
                return (string)base["Password"];
            }
            set
            {
                base["Password"] = value;
            }
        }

        [ConfigurationProperty("EnableSSL", IsRequired = false, DefaultValue = false)]
        public bool EnableSSL
        {
            get
            {
                return (bool)base["EnableSSL"];
            }
            set
            {
                base["EnableSSL"] = value;
            }
        }

        [ConfigurationProperty("DefaultFromAddress", IsRequired = false, DefaultValue = "")]
        public string DefaultFromAddress
        {
            get
            {
                return (string)base["DefaultFromAddress"];
            }
            set
            {
                base["DefaultFromAddress"] = value;
            }
        }

        [ConfigurationProperty("Timeout", IsRequired = false, DefaultValue = 15000)]
        public int Timeout
        {
            get
            {
                return (int)base["Timeout"];
            }
            set
            {
                base["Timeout"] = value;
            }
        }
    }


    public class EmailServerSetting
    {
        public string SMTPHost { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool EnableSSL { get; set; }
        public string DefaultFromAddress { get; set; }
    }
}
