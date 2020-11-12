using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Model
{
    public class EmailMessage
    {
        public bool IsBodyHTML { get; set; }

        /// <summary>
        /// From address
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Split by ,
        /// </summary>
        public string SendTo { get; set; }

        /// <summary>
        ///  Split by ,
        /// </summary>
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// Attachment file physical file path
        /// </summary>
        public List<AttachmentsDataInfo> Attachments { get; set; }

        public EmailMessage()
        {
            IsBodyHTML = false;
            From = string.Empty;
            SendTo = string.Empty;
            CC = string.Empty;
            Subject = string.Empty;
            Body = string.Empty;
            Attachments = new List<AttachmentsDataInfo>();
        }
    }
}
