using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Admin.BootstrapTree
{
    public class BootstrapTreeNode
    {
        public string text { get; set; }
        public string icon { get; set; }
        public string[] tags { get; set; }
        public List<BootstrapTreeNode> nodes { get; set; }

        public BootstrapTreeNode()
        {
            this.text = "";
            this.icon = "";
            this.tags = new string[] { };
            this.nodes = new List<BootstrapTreeNode>();
        }

        public BootstrapTreeNode(string text, string icon, string[] tags)
        {
            this.text = text;
            this.icon = icon;
            this.tags = tags;
            this.nodes = new List<BootstrapTreeNode>();
        }
    }
}