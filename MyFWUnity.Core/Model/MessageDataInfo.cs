using MyFWUnity.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Model
{
    public class MessageDataInfo : BaseDataModel<MessageDataInfo, B_Message>
    {
        //接受通知用户id
        public string ReceivedUserUniqueId { get; set; }
        public string SenderUserUniqueId { get; set; }
        //通知内容
        public string Body { get; set; }
        //是否已读 默认未读
        public int IsReceived { get; set; }
        //通知关联其他业务， Issue或者其他 不需要传 ，默认根据Soure来
        public string Type { get; set; }
        //其他业务ID
        public string EntityUniqueId { get; set; }

        [ForMember]
        public bool IsReceivedResutl
        {
            get { return IsReceived == 1; }
        }

        [ForMember]
        public string ReceivedUserName { get; set; }

        [ForMember]
        public string SenderUserName { get; set; }
    }
}
