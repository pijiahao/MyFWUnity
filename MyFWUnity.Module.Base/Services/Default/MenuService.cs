using MyFWUnity.Common;
using MyFWUnity.Common.Config;
using MyFWUnity.Core.Services;
using MyFWUnity.Module.Base.DataContracts;
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
    public class MenuService : IMenuService
    {
        private readonly string MenuFileName = "Menu.xml";
        private readonly string MenuKey = "Menu";
        [Dependency]
        public IPermissionService EntryRelationService { get; set; }
        public List<MenuDataInfo> GetMenuListData(List<string> permissionData)
        {
            List<MenuDataInfo> menuDataInfos = null;
            if (ApplicationHelper.Has(MenuKey))
            {
                if (CommonDefine.GetIsRefreshMenuOrPermission())
                {
                    menuDataInfos = LoadXmlMenuData();
                    if (menuDataInfos != null)
                    {
                        ApplicationHelper.Set(MenuKey, menuDataInfos);
                    }
                }
                else
                {
                    menuDataInfos = ApplicationHelper.Get(MenuKey) as List<MenuDataInfo>;
                }
            }
            else
            {
                menuDataInfos = LoadXmlMenuData();
                if (menuDataInfos != null)
                {
                    ApplicationHelper.Set(MenuKey, menuDataInfos);
                }
            }

            MenuDataLoadPermission(ref menuDataInfos, permissionData);

            return menuDataInfos;
        }

        /// <summary>
        /// 加载权限获取菜单数据
        /// </summary>
        /// <param name="menuDataInfos"></param>
        /// <param name="permissionData"></param>
        private void MenuDataLoadPermission(ref List<MenuDataInfo> menuDataInfos, List<string> permissionData)
        {
            foreach (var item in menuDataInfos)
            {
                if (string.IsNullOrEmpty(item.BindPermissionCode))
                {
                    if (item.Childrens != null)
                    {
                        item.IsShow = IsParentShow(item.Childrens, permissionData);
                        List<MenuDataInfo> _menuDataInfos = item.Childrens;
                        MenuDataLoadPermission(ref _menuDataInfos, permissionData);
                        item.Childrens = _menuDataInfos;
                    }
                    else
                    {
                        item.IsShow = true;
                    }
                }
                else
                {
                    if (permissionData.Contains(item.BindPermissionCode))
                    {
                        item.IsShow = true;
                    }
                }
            }
        }


        /// <summary>
        /// 父级菜单是否显示
        /// </summary>
        /// <param name="childrens"></param>
        /// <param name="permissionData"></param>
        /// <returns></returns>
        private bool IsParentShow(List<MenuDataInfo> childrens, List<string> permissionData)
        {
            int index = 0;
            foreach (var item in childrens)
            {
                if (string.IsNullOrEmpty(item.BindPermissionCode))
                {
                    index++;
                }
                else
                {
                    index = 0;
                    if (permissionData.Contains(item.BindPermissionCode))
                    {
                        index++;
                        break;
                    }
                }

            }
            return index > 0 || index == childrens.Count;
        }



        /// <summary>
        /// 获取菜单数据
        /// </summary>
        /// <returns></returns>
        private List<MenuDataInfo> LoadXmlMenuData()
        {
            List<MenuDataInfo> menuDataInfos = null;
            string folderPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SystemConfiguration");
            string entityFile = Path.Combine(folderPath, MenuFileName);
            if (File.Exists(entityFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(entityFile);
                try
                {
                    XmlNodeList xmlNodeList = xmlDoc.SelectNodes("//Menus");
                    foreach (XmlNode entityElement in xmlNodeList)
                    {
                        if (entityElement is XmlComment)
                            continue;
                        menuDataInfos = GetMenusByXmlNode(entityElement.ChildNodes);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return menuDataInfos;
        }

        private List<MenuDataInfo> GetMenusByXmlNode(XmlNodeList xmlNodeList)
        {
            List<MenuDataInfo> menuDataInfos = null;
            if (xmlNodeList.Count > 0)
            {
                menuDataInfos = new List<MenuDataInfo>();
                foreach (XmlNode entityElement in xmlNodeList)
                {
                    if (entityElement is XmlComment)
                        continue;
                    string name = string.Empty;
                    if (entityElement.Attributes["Name"] != null)
                    {
                        name = entityElement.Attributes["Name"].Value;
                    }
                    string icon = string.Empty;
                    if (entityElement.Attributes["Icon"] != null)
                    {
                        icon = entityElement.Attributes["Icon"].Value;
                    }
                    string href = string.Empty;
                    if (entityElement.Attributes["Href"] != null)
                    {
                        href = entityElement.Attributes["Href"].Value;
                    }
                    string bindPermissionCode = string.Empty;
                    if (entityElement.Attributes["BindPermissionCode"] != null)
                    {
                        bindPermissionCode = entityElement.Attributes["BindPermissionCode"].Value;
                    }
                    MenuDataInfo menuDataInfo = new MenuDataInfo()
                    {
                        BindPermissionCode = bindPermissionCode,
                        Href = href,
                        Icon = icon,
                        Name = name,
                        Childrens = GetMenusByXmlNode(entityElement.ChildNodes)
                    };
                    menuDataInfos.Add(menuDataInfo);
                }
            }
            return menuDataInfos;
        }


    }
}
