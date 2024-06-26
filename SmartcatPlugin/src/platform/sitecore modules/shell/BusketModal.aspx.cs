using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SmartcatPlugin.sitecore_modules.shell
{
    public partial class BusketModal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateTree();
            }
        }

        private void PopulateTree()
        {
            Database masterDb = Sitecore.Configuration.Factory.GetDatabase("core");
            Item rootItem = masterDb.GetItem("/sitecore");

            TreeNode rootNode = new TreeNode(rootItem.Name, rootItem.ID.ToString());
            AddChildNodes(rootItem, rootNode);
            TreeView1.Nodes.Add(rootNode);
        }

        private void AddChildNodes(Item parentItem, TreeNode parentNode)
        {
            foreach (Item child in parentItem.Children)
            {
                TreeNode childNode = new TreeNode(child.Name, child.ID.ToString());
                parentNode.ChildNodes.Add(childNode);

                AddChildNodes(child, childNode);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var selectedItems = new List<string>();

            foreach (TreeNode node in TreeView1.Nodes)
            {
                AddCheckedNodes(node, selectedItems);
            }
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