using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Entities
{
    /// <summary>
    /// Lớp đại diện thương hiệu sản phẩm
    /// </summary>
    public class Brand
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Constructors
        /// </summary>
        public Brand()
        {
            BrandId = 0;
            BrandName = "";
            Description = "";
        }

        public Brand(int id, string name, string desc)
        {
            BrandId = id;
            BrandName = name;
            Description = desc;
        }
    }
}