using CLMS.Business;
using CLMS.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.ManageSite
{
    public class ViewDatas
    {
        private static ViewDatas _instance;

        private ViewDatas()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            _sysName = Configuration["SysName"];
            //_sysName = new ConfigBusiness().GetSysConfigValueByName("SysName");            
        }

        public static ViewDatas GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ViewDatas();
            }
            return _instance;
        }

        private IConfigurationRoot Configuration { get; set; }
        private string _sysName { get; set; }

        public IEnumerable<SelectListItem> GetRoleList()
        {
            //IEnumerable<SelectListItem> select = new SelectList(new RoleBusiness().GetList(), "Id", "Name"); 

            List<SelectListItem> select = new List<SelectListItem>();
            foreach (var e in Enum.GetValues(typeof(UserType)))
            {
                select.Add(new SelectListItem()
                {
                    Text = ((UserType)e).GetDescription(),
                    Value = ((int)e).ToString()
                });
            }
            return select;
        }
        public IEnumerable<SelectListItem> GetLogTypeList()
        {
            //IEnumerable<SelectListItem> select = new SelectList(new RoleBusiness().GetList(), "Id", "Name"); 

            List<SelectListItem> select = new List<SelectListItem>();
            foreach (var e in Enum.GetValues(typeof(LogType)))
            {
                select.Add(new SelectListItem()
                {
                    Text = ((LogType)e).GetDescription(),
                    Value = ((int)e).ToString()
                });
            }

            return select;
        }
        public IEnumerable<SelectListItem> GetLineTypeList()
        {
            //IEnumerable<SelectListItem> select = new SelectList(new RoleBusiness().GetList(), "Id", "Name");             
            List<SelectListItem> select = new List<SelectListItem>();
            foreach (var e in Enum.GetValues(typeof(LineType)))
            {
                select.Add(new SelectListItem()
                {
                    Text = ((LineType)e).GetDescription(),
                    Value = ((int)e).ToString()
                });
            }
            return select;
        }
        public IEnumerable<SelectListItem> GetServiceProviderList()
        {
            //IEnumerable<SelectListItem> select = new SelectList(new RoleBusiness().GetList(), "Id", "Name");             
            List<SelectListItem> select = new List<SelectListItem>();
            foreach (var e in Enum.GetValues(typeof(ServiceProviderType)))
            {
                select.Add(new SelectListItem()
                {
                    Text = ((ServiceProviderType)e).GetDescription(),
                    Value = ((int)e).ToString()
                });
            }
            return select;
        }
        public IEnumerable<SelectListItem> GetOrganizationList()
        {
            IEnumerable<SelectListItem> select = new OrganBusiness().GetAllItemsByView().Select(n => new SelectListItem()
            {
                Text = string.Format("{0}-{1}", n.Name, string.IsNullOrEmpty(n.ParentName) ? "" : $"[{n.ParentName}]"),
                Value = n.Id.ToString()
            });
            return select;
        }
        public IEnumerable<SelectListItem> GetSMSTelbook()
        {
            IEnumerable<SelectListItem> select = new TelbookBusiness().GetList().Select(n => new SelectListItem()
            {
                Text = $"{n.Name}-{n.Telephone}",
                Value = n.Id.ToString()
            });
            return select;
        }

        public IEnumerable<SelectListItem> GetSmSTelbookSelected(string ids)
        {
            //string[] selectedIds = !string.IsNullOrEmpty(ids) ? ids.Split('-') : new string[] { };
            #region MyRegion
            //// IEnumerable<SelectListItem> select = GetSMSTelbook();
            //// if (selectedIds.Length>0)
            //// {
            ////     foreach (var item in select)
            ////     {
            ////         for (int i = 0; i < selectedIds.Length; i++)
            ////         {
            ////             if (item.Value == selectedIds[i])
            ////             {
            ////                 item.Selected = true;
            ////             }
            ////         }
            ////     }
            //// }  
            #endregion
            //List<SelectListItem> select = new List<SelectListItem>() { };
            #region MyRegion
            //foreach (var n in new TelbookBusiness().GetList())
            //{
            //    for (int i = 0; i < selectedIds.Length; i++)
            //    {
            //        if (n.Id.ToString() == selectedIds[i])
            //        {
            //            select.Add(new SelectListItem()
            //            {
            //                Text = $"{n.Name}-{n.Telephone}",
            //                Value = n.Id.ToString(),
            //                Selected = true
            //            });
            //        }
            //        else
            //        {
            //            select.Add(new SelectListItem()
            //            {
            //                Text = $"{n.Name}-{n.Telephone}",
            //                Value = n.Id.ToString()
            //            });
            //        }
            //    }
            //} 
            #endregion
            List<string> selectedIds = !string.IsNullOrEmpty(ids) ? ids.Split(',').ToList() : new List<string> { };
            List<SelectListItem> selectItem = new List<SelectListItem>() { };
            List<SelectListItem> allItem = new List<SelectListItem>() { };
            GetSMSTelbook().ToList().ForEach(n => allItem.Add(n));
            //找到并创建选中项
            allItem.ForEach(n =>
            {
                #region 1.0
                //selectedIds.ForEach(i =>
                //        {
                //            if (i == n.Value)
                //            {
                //                selectItem.Add(new SelectListItem()
                //                {
                //                    Text = n.Text,
                //                    Value = n.Value,
                //                    Selected = true
                //                });
                //        //allItem.Remove(n);
                //    }
                //        }); 
                #endregion
                if (selectedIds.Contains(n.Value))
                {
                    selectItem.Add(new SelectListItem()
                    {
                        Text = n.Text,
                        Value = n.Value,
                        Selected = true
                    });
                }
            });
            allItem.RemoveAll(n => selectedIds.Contains(n.Value));
            selectItem.ForEach(n => allItem.Add(n));
            return allItem;
        }
        public TagBuilder GetOrganSubMeun(string action, Dictionary<string, string> htmlAttributes = null)
        {
            TagBuilder root = new TagBuilder("ul");
            root.AddCssClass("dropdown-menu");
            if (htmlAttributes != null)
            {
                foreach (var attribute in htmlAttributes)
                {
                    root.Attributes.Add(attribute);
                }
            }
            var items = new OrganBusiness().GetAllItems();
            if (items.Count() > 0)
            {
                var children = items.Where(n => n.ParentId == 0);
                foreach (var child in children)
                {
                    root.InnerHtml.AppendHtml(GetOrganNode(child, items, action, htmlAttributes));
                }

            }
            return root;
        }

        public TagBuilder GetOrganChildrenSubMeun(int organId, string action, Dictionary<string, string> htmlAttributes = null)
        {
            OrganModel item;
            if (organId == 0)
            {
                return GetOrganSubMeun(action, htmlAttributes);
            }
            else
            {
                item = new OrganBusiness().GetItem(organId);

                TagBuilder root = new TagBuilder("ul");
                root.AddCssClass("dropdown-menu");
                if (htmlAttributes != null)
                {
                    foreach (var attribute in htmlAttributes)
                    {
                        root.Attributes.Add(attribute);
                    }
                }
                if (item != null)
                {
                    var items = new OrganBusiness().GetChildren(organId);
                    if (items.Count() > 0 && item != null)
                    {
                        var children = items.Where(n => n.ParentId == item.ParentId);
                        foreach (var child in children)
                        {
                            root.InnerHtml.AppendHtml(GetOrganNode(child, items, action, htmlAttributes));
                        }
                    }
                }

                return root;
            }

        }


        private TagBuilder GetOrganNode(OrganModel item, List<OrganModel> items, string action, Dictionary<string, string> htmlAttributes)
        {
            TagBuilder node;
            var children = items.Where(n => n.ParentId == item.Id);
            if (children.Count() > 0)
            {
                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("dropdown-submenu");
                TagBuilder a = new TagBuilder("a");
                a.Attributes.Add("tabindex", "0");
                a.InnerHtml.Append(item.Name);

                //删除下面这一行就只能选择根节点
                a.Attributes.Add("onclick", $"{action}('{item.Id}','{item.Name}')");

                TagBuilder ul = new TagBuilder("ul");
                if (htmlAttributes != null)
                {
                    foreach (var attribute in htmlAttributes)
                    {
                        ul.Attributes.Add(attribute);
                    }
                }
                ul.AddCssClass("dropdown-menu");
                foreach (var child in children)
                {
                    ul.InnerHtml.AppendHtml(GetOrganNode(child, items, action, htmlAttributes));
                }
                li.InnerHtml.AppendHtml(ul);
                li.InnerHtml.AppendHtml(a);
                node = li;
            }
            else
            {
                TagBuilder li = new TagBuilder("li");
                TagBuilder a = new TagBuilder("a");
                a.Attributes.Add("tabindex", "0");
                a.Attributes.Add("onclick", $"{action}('{item.Id}','{item.Name}')");
                a.InnerHtml.Append(item.Name);
                li.InnerHtml.AppendHtml(a);
                node = li;
            }
            return node;
        }

        public UserModel GetUser(HttpRequest request)
        {
            return CookieHelper.GetInstance(HelperKeys.UserIdCookieKey).GetCookie<UserModel>(request);
        }



        public string GetSysName()
        {
            return _sysName;
        }

        public string GetDicValue(string key, Dictionary<string, string> dic)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

