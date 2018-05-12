using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemoryAudio.Libs;
using MemoryAudio.Models.Context;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Admin.BootstrapTree
{
    public class BootstrapTree
    {
        public List<BootstrapTreeNode> nodes { get; set; }

        public BootstrapTree()
        {
            this.nodes = new List<BootstrapTreeNode>();
        }

        public static BootstrapTree LoadCategoryBootstrapTree()
        {
            try
            {
                var bsTree = new BootstrapTree();
                using (var db = new DBContext())
                {
                    var parentCategories = db.Categories
                        .Where(r => r.ParentId == null)
                        .OrderBy(r => r.SortIdx)
                        .ThenBy(r => r.CategoryName)
                        .ToList();
                    foreach (var cat in parentCategories)
                    {
                        var parent = new BootstrapTreeNode();
                        parent.text = cat.CategoryName;
                        parent.icon = "fa fa-folder";
                        parent.tags = new string[] { cat.CategoryId.ToString() };
                        AppendChildNodes(parent);
                        bsTree.nodes.Add(parent);
                    }
                }
                return bsTree;
            }
            catch (Exception ex)
            {
                // Write error logs
                EventLogs.Write("BootstrapTree - GetTree: " + ex.ToString());
                throw ex;
            }
        }

        public static void AppendChildNodes(BootstrapTreeNode parent)
        {
            if (parent != null)
            {
                var parentId = 0;
                if (int.TryParse(parent.tags[0], out parentId))
                {
                    try
                    {
                        using (var db = new DBContext())
                        {
                            var childCategories = db.Categories.Where(r => r.ParentId == parentId).OrderBy(r => r.SortIdx).ThenBy(r => r.CategoryName).ToList();
                            foreach (var cat in childCategories)
                            {
                                var child = new BootstrapTreeNode();
                                child.text = cat.CategoryName;
                                child.icon = "fa fa-folder";
                                child.tags = new string[] { cat.CategoryId.ToString() };
                                AppendChildNodes(child);
                                parent.nodes.Add(child);
                            }
                            if (childCategories.Count > 0)
                            {
                                parent.text += " (" + childCategories.Count + ")";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Write error logs
                        EventLogs.Write("BootstrapTree - AppendChildNodes: " + ex.ToString());
                        throw ex;
                    }
                    //.try
                }
                //.if
            }
            //.if
        }
    }
}