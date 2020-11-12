using MyFWUnity.Core.Model;
using MyFWUnity.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.DataContracts
{
    public class EmailMessageDataInfo : BaseDataModel<EmailMessageDataInfo, B_EmailMessage>
    {

        public string SendTo { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReceiveUsers { get; set; }
        public string CCUsers { get; set; }
        public bool IsBodyHTML { get; set; }

        [ForMember]
        public string From { get; set; }

        [ForMember]
        public Dictionary<string, string> ReceiveUserNames { get; set; }

        [ForMember]
        public Dictionary<string, string> CCUserNames { get; set; }


    }
}
