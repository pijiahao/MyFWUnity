using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.WebApp.Infrastructure.Model.User
{
    public class UserLoginInfoViewModel
    {
        public UserLoginInfoViewModel()
        {
            UserID = 0;
            UserName = string.Empty;
        }
        public int UserID { get; set; }
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string UserFace { get; set; }

    }
}
