using MyFWUnity.Common.Config;
using MyFWUnity.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Infrastructure.Email
{
    public interface IEmailService
    {
        EmailServerSettingSection EmailServerConfig { get; set; }
        bool SendEmail(EmailMessage msg);
    }
}
