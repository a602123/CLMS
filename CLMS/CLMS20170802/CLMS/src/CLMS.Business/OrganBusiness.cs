using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLMS.Model;
using CLMS.DataProvider;

namespace CLMS.Business
{
    public class OrganBusiness
    {
        private OrganDataProvider _provider;

        public OrganBusiness()
        {
            _provider = new OrganDataProvider();
        }

        public object GetPage(string name, int? state, int? typeId, int limit, int offset)
        {
            string condition = new ConditionHelper().And("Name", ref name, CompareType.Like)
                                       .And("State", state, CompareType.Equal)
                                       .ToString();
            var searchObj = new
            {
                Name = name,
                State = state
            };
            return _provider.GetAllItems(condition, searchObj, " order by Id ASC");
        }

        public List<OrganModel> GetAllItems()
        {
            var items = _provider.GetAllItems("", null, " order by Id ASC");
            return items;
        }

        /// <summary>
        /// 获取部门地址及状态
        /// </summary>
        /// <param name="id">父部门Id，空为全部</param>
        /// <returns></returns>
        public List<OrganModel> GetPosition(int? id)
        {
            List<OrganModel> items;
            if (id == null)
            {
                items = _provider.GetPosition("", null);
            }
            else
            {
                var organs = GetChildren(id.Value).Select(n => n.Id).ToArray();
                string condition = new ConditionHelper().And("Id", organs, CompareType.In).ToString();
                var searchObj = new { Id = organs };
                items = _provider.GetPosition(condition, searchObj);
            }
            return items;
        }

        public List<OrganModel> GetAllItemsByView()
        {
            var items = _provider.GetAllItemsByView("", null, " order by Id ASC");
            return items;
        }

        public IEnumerable<OrganModel> GetOrganhasLines()
        {
            string sql = "select o.Id,o.`Name`,COUNT(o.Id) as 'Linecount' from tb_organization as o INNER  JOIN tb_line as l on o.Id=l.OrganizationId GROUP BY o.Id";
            return GetBySql(sql, null);
        }

        public IEnumerable<OrganModel> GetBySql(string sql, object searchObj)
        {
            return _provider.GetListBySql(sql, searchObj);
        }

        public void Insert(OrganModel model)
        {
            _provider.Insert(model);
        }

        public OrganModel GetItem(int id)
        {
            string condition = "AND Id = @Id";
            return _provider.GetItem(condition, new { Id = id });
        }

        public OrganModel GetParentNode()
        {
            string condition = "AND ParentId=@ParentId";
            return _provider.GetItemByCondition(condition, new { ParentId = 0 });
        }

        public void Update(OrganModel model)
        {
            string condition = "AND Id = @Id";
            var item = _provider.GetItem(condition, new { Id = model.Id });
            if (item == null)
            {
                throw new Exception("所要修改的信息不存在，请重试");
            }
            _provider.Update(condition, model);
        }

        public void SetPosition(int id, string x, string y)
        {
            string condition = "AND Id=@Id";
            var item = _provider.GetItem(condition, new { Id = id });
            if (item == null)
            {
                throw new Exception("所要修改的信息不存在，请重试");
            }
            item.X = x;
            item.Y = y;
            _provider.Update(condition, item);
        }

        public void Ban(int id)
        {
            string condition = "AND Id = @Id";
            var item = _provider.GetItem(condition, new { Id = id });
            if (item == null)
            {
                throw new Exception("所要修改的信息不存在，请重试");
            }
            item.State = 0;
            _provider.ChangeState(condition, item);
        }

        public void Del(int id)
        {
            string condition = "AND ParentId = @ParentId";
            var searchObj = new
            {
                ParentId = id
            };
            List<OrganModel> result = _provider.GetAllItems(condition, searchObj, "");
            if (result.Count > 0)
            {
                throw new Exception("请先删除该节点下的所有子节点！");
            }
            _provider.Del(" AND Id =@Id ", id);
        }

        public List<SelectTreeModel> GetListForSelect()
        {
            List<OrganModel> list = GetAllItems();
            SelectTreeModel root = new SelectTreeModel()
            {
                id = 0,
                text = "所有"
            };
            return GetNode(root, list).nodes;
        }

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<SelectTreeModel> GetListForSelect(int id)
        {
            List<OrganModel> list = GetAllItems();
            SelectTreeModel root = new SelectTreeModel()
            {
                id = id,
                text = list.Find(value => value.Id == id).Name
            };
            var result = GetNode(root, list).nodes;
            return result;
        }

        public SelectTreeModel GetNode(SelectTreeModel root, List<OrganModel> items)
        {
            var children = items.Where(n => n.ParentId == root.id);
            if (children.Count() > 0)
            {
                root.nodes = new List<Business.SelectTreeModel>();
                foreach (var child in children)
                {
                    root.nodes.Add(GetNode(new SelectTreeModel()
                    {
                        id = child.Id,
                        text = child.Name
                    }, items));
                }
            }
            else
            {
                root.href = root.id;
            }
            return root;
        }

        /// <summary>
        /// 获取所有子节点，包括自身
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<OrganModel> GetChildren(int id)
        {
            List<OrganModel> resultList = new List<OrganModel>();
            List<OrganModel> fullList = GetAllItems();
            resultList = GetChildrenList(resultList, fullList, id);
            resultList.Add(fullList.Find(n => n.Id == id));
            return resultList;
        }

        private List<OrganModel> GetChildrenList(List<OrganModel> list, List<OrganModel> items, int id)
        {
            IEnumerable<OrganModel> children = items.Where(n => n.ParentId == id);
            if (children.Count() > 0)
            {
                foreach (var child in children)
                {
                    list.Add(child);
                    GetChildrenList(list, items, child.Id);
                }
            }
            return list;
        }

        public TreeViewModel GetTreeView(int organId)
        {
            List<OrganModel> list = GetAllItems();
            TreeViewModel root = new TreeViewModel()
            {
                id = organId,
                text = list.Find(value => value.Id == organId).Name,

            };
            GetTreeViewNode(root, list);
            return root;
        }

        public TreeViewModel GetTreeViewNode(TreeViewModel root, List<OrganModel> items)
        {
            var children = items.Where(n => n.ParentId == root.id);
            if (children.Count() > 0)
            {
                root.children = new List<TreeViewModel>();
                foreach (var child in children)
                {
                    root.children.Add(GetTreeViewNode(new TreeViewModel()
                    {
                        id = child.Id,
                        text = child.Name
                    }, items));
                }
            }
            return root;
        }
    }
    public class SelectTreeModel
    {
        public int href { get; set; }
        public int id { get; set; }
        public string text { get; set; }
        public List<SelectTreeModel> nodes { get; set; }
    }
    public class SubmenuModel
    {
        public List<SelectTreeModel> items { get; set; }
    }

    public class TreeViewModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public string imgUrl { get; set; }
        public List<TreeViewModel> children { get; set; }
    }


}
