using MyFWUnity.Module.Base.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.Services.Interfaces
{
    public interface IEmailMessageService
    {
        void SendMsg(EmailMessageDataInfo emailMessageDataInfo);
        List<EmailMessageDataInfo> QueryPage(string condition, int pageIndex, int pageSize, out long recordCount);
    }
}
