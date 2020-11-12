using MyFWUnity.Common;
using MyFWUnity.Common.Config;
using MyFWUnity.Common.Encrypt;
using MyFWUnity.Core.Model;
using MyFWUnity.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Module.Base.DataContracts
{
    public class UserDataInfo : BaseDataModel<UserDataInfo, B_User>
    {

        public UserDataInfo()
        {

        }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Name
        {
            get
            {
                return (string.IsNullOrEmpty(UserName) ? UserCode : UserName);
            }
        }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserFace { get; set; }
        //1 管理员 2普通用户  0则是默认值
        public int IsAdministrator { get; set; }

        public bool IsAdmin { get { return (IsAdministrator == 1); } }

        public System.DateTime LastLoginDate { get; set; }

        [ForMember]
        public string NewPassword { get; set; }

        [ForMember]
        public string OldPassword { get; set; }


        protected override void StoreUnchangedFieldsForAdd()
        {
            if (string.IsNullOrEmpty(this.Password))
            {
                this.Password = CommonDefine.GetDefaultUserPassword();
            }
            this.Password = EncryptManager.Encode(this.Password);
            this.CreateDate = DateTime.Now;
            this.LastLoginDate = DateTime.Now;
        }

        protected override void StoreUnchangedFieldsForUpdate(B_User entity)
        {
            if (!string.IsNullOrEmpty(OldPassword))
            {
                string op = EncryptManager.Encode(this.OldPassword);
                if (!op.Equals(entity.Password))
                {
                    throw new ValidatebjectException("密码验证错误");
                }
                else
                {
                    this.Password = this.NewPassword;
                }
            }
            if (string.IsNullOrEmpty(Password))
            {
                this.Password = entity.Password;
            }
            else
            {
                this.Password = EncryptManager.Encode(this.Password);
            }
            if (string.IsNullOrEmpty(UserName))
            {
                this.UserName = entity.UserName;
            }
            if (string.IsNullOrEmpty(Email))
            {
                this.Email = entity.Email;
            }
            if (IsAdministrator == 0)
            {
                this.IsAdministrator = entity.IsAdministrator;
            }
            this.UserCode = entity.UserCode;
            this.CreateDate = entity.CreateDate;
            this.LastLoginDate = entity.LastLoginDate;
        }
    }
}
