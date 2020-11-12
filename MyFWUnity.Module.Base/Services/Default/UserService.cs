using MyFWUnity.Common;
using MyFWUnity.Common.Encrypt;
using MyFWUnity.Common.Services;
using MyFWUnity.Core.Model.DataContracts;
using MyFWUnity.Core.Repositories;
using MyFWUnity.Core.Services;

using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.Module.Base.Services.Interfaces;
using MyFWUnity.Module.Base.Repositories.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyFWUnity.DataAccess.Entity;

namespace MyFWUnity.Module.Base.Services.Default
{
    public class UserService : BaseService, IUserService
    {
        public UserService() : base()
        {
        }

        [Dependency]
        public IUserRepository UserRepository { get; set; }

        /// <summary>
        /// 初始化创建管理员用户
        /// </summary>
        public void CreateAdminUser()
        {
            B_User user = UserRepository.FirstOrDefault(n => n.UserCode == "admin");
            if (user == null)
            {
                user = new B_User();
                user.CreateDate = DateTime.Now;
                user.ID = Guid.NewGuid().ToString();
                user.UserCode = "admin";
                user.Password = EncryptManager.Encode("admin");
                user.IsAdministrator = 1;
                user.LastLoginDate = DateTime.Now;
                UserRepository.Add(user);
                this.Context.Save();
            }
        }

        /// <summary>
        /// 根据UserID获取用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserDataInfo GetUserByID(string userID)
        {
            B_User user = UserRepository.FirstOrDefault(n => n.ID == userID);
            UserDataInfo userDataInfo = ToolsEx.GetInstance<UserDataInfo>().MapTo(user);
            return userDataInfo;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserDataInfo GetUserDataInfoByLogin(string userCode, string password)
        {
            B_User user = UserRepository.FirstOrDefault(n => n.UserCode == userCode && n.Password == password);
            UserDataInfo userDataInfo = ToolsEx.GetInstance<UserDataInfo>().MapTo(user);
            if (userDataInfo != null)
            {
                user.LastLoginDate = DateTime.Now;
                UserRepository.Update(user);
                this.Context.Save();
                LogAdd(new LogDataInfo()
                {
                    ModulePage = "Login",
                    Description = "{0}登录了系统",
                }, user.ID);
            }
            return userDataInfo;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<UserDataInfo> QueryPage(string condition, int pageIndex, int pageSize, out long recordCount)
        {
            IList<B_User> list = null;
            Expression<Func<B_User, bool>> expression = null;
            if (!string.IsNullOrEmpty(condition))
            {
                expression = n => n.UserName.Contains(condition) || n.UserCode.Contains(condition);
            }
            list = UserRepository.LoadPageList(out recordCount, pageIndex, pageSize, expression, n => n.CreateDate, false);
            if (list == null)
            {
                return null;
            }
            return ToolsEx.GetInstance<UserDataInfo>().ToConvertForMember(list);
        }

        /// <summary>
        /// 模糊匹配用户名和用户账号查询用户id集合
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string[] GetUserIDsByCondition(string condition)
        {
            string[] strArray = new string[] { "" };
            IList<B_User> list = UserRepository.FindList(n => n.UserCode.Contains(condition) || n.UserName.Contains(condition));
            if (list != null)
            {
                strArray = list.Select(n => n.ID).ToArray();
            }
            return strArray;

        }


    }
}
