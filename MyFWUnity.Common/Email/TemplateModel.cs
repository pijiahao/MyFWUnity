using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common.Email
{
    public class TemplateModel
    {
        public string UserFace { get; set; }
        public string UserHref { get; set; }

        public string UserName { get; set; }
        public string Title { get; set; }
        public string TitleHerf { get; set; }
        public string Number { get; set; }
        public string NumberHerf { get; set; }

        public string CloseStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string Operation { get; set; }

        public string Date { get; set; }

        public string PlatformName { get; set; }

    }
}
