using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Entities
{
    /// <summary>
    /// Lớp đại diện danh mục sản phẩm
    /// </summary>
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public int SortIdx { get; set; }

        /// <summary>
        /// Constructors
        /// </summary>
        public Category()
        {
            CategoryId = 0;
            CategoryName = "";
            Description = "";
            ParentId = null;
            SortIdx = int.MaxValue;
        }

        public Category(int id, string name, string desc)
        {
            CategoryId = id;
            CategoryName = name;
            Description = desc;
            ParentId = null;
            SortIdx = int.MaxValue;
        }
    }
}