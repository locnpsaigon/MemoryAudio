using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemoryAudio.Libs;
using MemoryAudio.Models.Context;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Admin.CategoryTree
{
    public class CategoryTree
    {
        public List<CategoryTreeNode> Nodes;

        public CategoryTree()
        {
            Nodes = new List<CategoryTreeNode>();
        }

        public static CategoryTree GetCategoryTree()
        {
            try
            {
                var categoryTree = new CategoryTree();
                using (var db = new DBContext())
                {
                    var parentCategories = db.Categories.Where(r => r.ParentId == null).OrderBy(r => r.SortIdx).ThenBy(r => r.CategoryName).ToList();
                    foreach (var cat in parentCategories)
                    {
                        var parent = new CategoryTreeNode();
                        parent.CategoryId = cat.CategoryId;
                        parent.CategoryName = cat.CategoryName;
                        parent.Description = cat.Description;
                        parent.Parent = null;
                        parent.Level = 0;
                        parent.Nodes = new List<CategoryTreeNode>();
                        AppendChildNodes(parent);
                        categoryTree.Nodes.Add(parent);
                    }
                }
                return categoryTree;
            }
            catch (Exception ex)
            {
                // Write error logs
                EventLogs.Write("BootstrapTree - GetCategoryTree: " + ex.ToString());
                throw ex;
            }
        }

        public static void AppendChildNodes(CategoryTreeNode parent)
        {
            if (parent != null)
            {
                using (var db = new DBContext())
                {
                    var childCategories = db.Categories.Where(r => r.ParentId == parent.CategoryId).OrderBy(r => r.SortIdx).ThenBy(r => r.CategoryName).ToList();
                    foreach (var cat in childCategories)
                    {
                        var child = new CategoryTreeNode();
                        child.CategoryId = cat.CategoryId;
                        child.CategoryName = cat.CategoryName;
                        child.Description = cat.Description;
                        child.Parent = parent;
                        child.Level = parent.Level + 1;
                        child.Nodes = new List<CategoryTreeNode>();
                        AppendChildNodes(child);
                        parent.Nodes.Add(child);
                    }
                }
            }
        }

        public static void AppendParentNodes(CategoryTreeNode child)
        {
            if (child != null)
            {
                using(var db = new DBContext())
                {
                    var childCategory = db.Categories.Where(r => r.CategoryId == child.CategoryId).FirstOrDefault();
                    if (childCategory != null && childCategory.ParentId != null)
                    {
                        var parentCategory = db.Categories.Where(r => r.CategoryId == childCategory.ParentId).FirstOrDefault();
                        if (parentCategory != null)
                        {
                            var parent = new CategoryTreeNode();
                            parent.CategoryId = parentCategory.CategoryId;
                            parent.CategoryName = parentCategory.CategoryName;
                            parent.Description = parentCategory.Description;
                            parent.Parent = null;
                            child.Parent = parent;
                            AppendParentNodes(parent);
                        }
                    }
                }
            }
        }

        public List<CategoryTreeNode> ToList()
        {
            var nodes = new List<CategoryTreeNode>();
            foreach (var parent in Nodes)
            {
                AppendListNodes(nodes, parent);
            }
            return nodes;
        }

        public void AppendListNodes(List<CategoryTreeNode> list, CategoryTreeNode node)
        {
            if (list != null && node != null)
            {
                list.Add(node);
                foreach(var child in node.Nodes)
                {
                    AppendListNodes(list, child);
                }
            }
        }
    }
}