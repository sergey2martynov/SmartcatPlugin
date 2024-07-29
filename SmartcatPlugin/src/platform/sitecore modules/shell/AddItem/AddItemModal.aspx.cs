using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Sitecore.Data.Items;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Extensions;

namespace SmartcatPlugin.sitecore_modules.shell.AddItem
{
    public partial class AddItemModal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateTree();
            }

            if (IsPostBack)
            {
                Sitecore.Context.ClientPage.SendMessage((object)this, "item:refresh");
                Sitecore.Context.ClientPage.ClientResponse.Timer("item:refresh", 500);
            }
        }

        private void PopulateTree()
        {
            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var rootItem = masterDb.GetItem("/sitecore/content");

            var rootNode = new TreeNode(rootItem.Name, rootItem.ID.ToString())
            {
                Expanded = false
            };
            AddChildNodes(rootItem, rootNode);
            TreeView1.Nodes.Add(rootNode);
        }

        private void AddChildNodes(Item parentItem, TreeNode parentNode)
        {
            foreach (Item child in parentItem.Children)
            {
                TreeNode childNode = new TreeNode(child.Name, child.ID.ToString(), child.Appearance.GetIconPath());
                childNode.Expanded = false;

                if (child.IsFolder() || child.Fields.All(f => f.Name.StartsWith("_")))
                {
                    childNode.ShowCheckBox = false;
                }

                parentNode.ChildNodes.Add(childNode);

                AddChildNodes(child, childNode);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var itemIds = new List<string>();
            foreach (TreeNode chekedNode in TreeView1.CheckedNodes)
            {
                if (chekedNode.Value != null)
                {
                    itemIds.Add(chekedNode.Value);
                }
            }

            var serializedList = JsonConvert.SerializeObject(itemIds);
            CustomCacheManager.SetCache("selectedItems", serializedList);
        }

        private void AddCheckedNodes(TreeNode node, List<string> selectedItems)
        {
            if (node.Checked)
            {
                selectedItems.Add(node.Value);
            }

            foreach (TreeNode childNode in node.ChildNodes)
            {
                AddCheckedNodes(childNode, selectedItems);
            }
        }
    }
}