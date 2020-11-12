using MyFWUnity.Common;
using MyFWUnity.Common.Config;
using MyFWUnity.Core.Repositories;
using MyFWUnity.Core.Services;

using MyFWUnity.Module.Base.DataContracts;
using MyFWUnity.DataAccess.Entity;
using MyFWUnity.Module.Base.Services.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MyFWUnity.Module.Base.Services.Default
{
    public class PermissionService : IPermissionService
    {
        public PermissionService()
        {
            EntryRelationCommonService = new CommonService<EntryRelationDataInfo, B_EntryRelation>();
        }
        private readonly string PermissionFileName = "Permission.xml";
        private readonly string PermissionKey = "Permission";

        [Dependency]
        public IEntryRelationService EntryRelationService { get; set; }



        public CommonService<EntryRelationDataInfo, B_EntryRelation> EntryRelationCommonService { get; set; }

        /// <summary>
        /// 添加单个权限关联用户ID
        /// </summary>
        /// <param name="permissionID"></param>
        /// <param name="userID"></param>
        public void AddRelationByPermission(string permissionID, string userID,string projectID="")
        {
            EntryRelationCommonService.Add(new EntryRelationDataInfo()
            {
                EntryID = permissionID,
                EntryType = EntryType.Permission.ToString(),
                RelationID = userID,
                RelationType = RelationType.User.ToString(),
                ProjectID=(string.IsNullOrEmpty(projectID)?null:projectID)
            });
        }

        /// <summary>
        /// 添加多个权限关联用户ID
        /// </summary>
        /// <param name="permissionIDs"></param>
        /// <param name="userID"></param>
        public void AddRelationByPermissions(string[] permissionIDs, string userID, string projectID = "")
        {
            List<EntryRelationDataInfo> entryRelationDatas = new List<EntryRelationDataInfo>();
            foreach (var permissionID in permissionIDs)
            {
                entryRelationDatas.Add(new EntryRelationDataInfo()
                {
                    EntryID = permissionID,
                    EntryType = EntryType.Permission.ToString(),
                    RelationID = userID,
                    RelationType = RelationType.User.ToString(),
                    ProjectID = (string.IsNullOrEmpty(projectID) ? null : projectID)
                });
            }
            EntryRelationCommonService.Add(entryRelationDatas);
        }

        /// <summary>
        /// 获取所有权限列表
        /// </summary>
        /// <returns></returns>
        public List<PermissionDataInfo> GetPermissionListData()
        {
            List<PermissionDataInfo> permissionDataInfos = null;
            List<PermissionClass> permissionClasses = GetPermissionClassData();
            if (permissionClasses != null)
            {
                permissionDataInfos = new List<PermissionDataInfo>();
                foreach (var item in permissionClasses)
                {
                    permissionDataInfos.AddRange(item.PermissionDataInfos);
                }
            }
            return permissionDataInfos;
        }

        /// <summary>
        /// 获取权限分类列表
        /// </summary>
        /// <returns></returns>
        public List<PermissionClass> GetPermissionClassData()
        {
            List<PermissionClass> permissionClasses = null;
            if (ApplicationHelper.Has(PermissionKey))
            {
                if (CommonDefine.GetIsRefreshMenuOrPermission())
                {
                    permissionClasses = LoadXmlPermissionData();
                    if (permissionClasses != null)
                    {
                        ApplicationHelper.Set(PermissionKey, permissionClasses);
                    }
                }
                else
                {
                    permissionClasses = ApplicationHelper.Get(PermissionKey) as List<PermissionClass>;
                }
            }
            else
            {
                permissionClasses = LoadXmlPermissionData();
                if (permissionClasses != null)
                {
                    ApplicationHelper.Set(PermissionKey, permissionClasses);
                }
            }
            return permissionClasses;
        }

        /// <summary>
        /// 加载xml 初始化权限数据
        /// </summary>
        /// <returns></returns>
        private List<PermissionClass> LoadXmlPermissionData()
        {
            List<PermissionClass> permissionClasses = null;
            string folderPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SystemConfiguration");
            string entityFile = Path.Combine(folderPath, PermissionFileName);
            if (File.Exists(entityFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(entityFile);
                try
                {
                    XmlNodeList xmlNodeList = xmlDoc.SelectNodes("//Permissions");
                    permissionClasses = new List<PermissionClass>();
                    foreach (XmlNode entityElement in xmlNodeList)
                    {
                        if (entityElement is XmlComment)
                            continue;
                        xmlNodeList = entityElement.ChildNodes;
                        foreach (XmlNode permissionClass in xmlNodeList)
                        {
                            string name = permissionClass.Attributes["Name"].Value + string.Empty;
                            xmlNodeList = permissionClass.ChildNodes;
                            List<PermissionDataInfo> permissionDataInfos = new List<PermissionDataInfo>();
                            foreach (XmlNode permissions in xmlNodeList)
                            {
                                string _name = permissions.Attributes["Name"].Value + string.Empty;
                                string type = permissions.Attributes["Type"].Value + string.Empty;
                                string code = permissions.Attributes["Code"].Value + string.Empty;
                                permissionDataInfos.Add(new PermissionDataInfo()
                                {
                                    Name = _name,
                                    Type = type,
                                    Code = code,
                                });
                            };
                            permissionClasses.Add(new PermissionClass()
                            {
                                Name = name,
                                PermissionDataInfos = permissionDataInfos
                            });

                        }
                        break;
                    }
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return permissionClasses;
        }

        /// <summary>
        /// 获取用户所有权限列表信息
        /// </summary>
        /// <returns></returns>
        public List<PermissionDataInfo> QueryAllPermissionByUserID(string userID, string projectID = "", bool isAdministrator = false)
        {
            Dictionary<string, string> permissionIDs = new Dictionary<string, string>();
            if (!isAdministrator)
            {
                permissionIDs = EntryRelationService.GetEntryIDs(EntryType.Permission.ToString(), RelationType.User.ToString(), userID, projectID);
            }
            List<PermissionDataInfo> permissionDataInfos = GetPermissionListData();
            if (permissionDataInfos != null && (permissionIDs.Keys.Count > 0 || isAdministrator))
            {

                //permissionDataInfos = permissionDataInfos.Where(n => permissionIDs.Keys.Contains(n.Code)).ToList();
                foreach (var item in permissionDataInfos)
                {
                    if (isAdministrator)
                    {
                        item.IsChecked = true;
                    }
                    else
                    {
                        if (permissionIDs.ContainsKey(item.Code))
                        {
                            item.EntryRelationID = permissionIDs[item.Code];
                        }
                        item.IsChecked = permissionIDs.Keys.Contains(item.Code);

                    }

                }
            }
            return permissionDataInfos;
        }




        /// <summary>
        /// 获取用户所有权限列表信息
        /// </summary>
        /// <returns></returns>
        public List<PermissionClass> QueryAllPermissionClass()
        {
            List<PermissionClass> permissionDataInfos = GetPermissionClassData();
            foreach (var item in permissionDataInfos)
            {
                item.IsProjectShow = (item.PermissionDataInfos.Where(n => n.Type == "Project").Count() > 0);
                item.IsSystemShow = (item.PermissionDataInfos.Where(n => n.Type == "System").Count() > 0);
            }
            return permissionDataInfos;
        }

        /// <summary>
        /// 获取用户所有权限列表信息
        /// </summary>
        /// <returns></returns>
        public List<PermissionClass> QueryAllPermissionClassByUserID(string userID, string projectID = "")
        {
            Dictionary<string, string> permissionIDs = EntryRelationService.GetEntryIDs(EntryType.Permission.ToString(), RelationType.User.ToString(), userID, projectID);
            List<PermissionClass> permissionDataInfos = GetPermissionClassData();
            if (permissionDataInfos != null && permissionIDs.Keys.Count > 0)
            {
                //permissionDataInfos = permissionDataInfos.Where(n => permissionIDs.Keys.Contains(n.Code)).ToList();
                foreach (var item in permissionDataInfos)
                {
                    foreach (var _item in item.PermissionDataInfos)
                    {
                        _item.IsChecked = permissionIDs.Keys.Contains(_item.Code);
                        _item.EntryRelationID = permissionIDs[_item.Code];
                    }

                }
            }
            return permissionDataInfos;
        }
    }
}
