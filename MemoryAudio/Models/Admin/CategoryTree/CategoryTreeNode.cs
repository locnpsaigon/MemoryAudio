using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Admin.CategoryTree
{
    public class CategoryTreeNode
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public CategoryTreeNode Parent { get; set; }
        public int Level { get; set; }
        public List<CategoryTreeNode> Nodes { get; set; }

        public CategoryTreeNode()
        {
            CategoryId = 0;
            CategoryName = "";
            Description = "";
            Parent = null;
            Level = 0;
            Nodes = new List<CategoryTreeNode>();
        }

        public List<CategoryTreeNode> GetChildNodes()
        {
            var listNodes = new List<CategoryTreeNode>();
            AppendChildNodes(listNodes, this);
            return listNodes;
        }
        
        public void AppendChildNodes(List<CategoryTreeNode> listNodes, CategoryTreeNode parentNode)
        {
            if (listNodes == null) listNodes = new List<CategoryTreeNode>();
            if (parentNode != null)
            {
                listNodes.Add(parentNode);
                foreach(var childNode in parentNode.Nodes)
                {
                    AppendChildNodes(listNodes, childNode);
                }
            }
        }

        public List<CategoryTreeNode> GetParentNodes()
        {
            var listNodes = new List<CategoryTreeNode>();
            AppendParentNode(listNodes, this);
            listNodes.Reverse();
            return listNodes;
        }

        public void AppendParentNode(List<CategoryTreeNode> listNodes, CategoryTreeNode childNode)
        {
            if (listNodes == null) listNodes = new List<CategoryTreeNode>();
            if (childNode != null)
            {
                listNodes.Add(childNode);
                if (childNode.Parent != null)
                {
                    AppendParentNode(listNodes, childNode.Parent);
                }
            }
        }
    }
}