using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Home
{
    public class ProductViewModel
    {
        public ProductInfo Product { get; set; }
        public Category Category { get; set; }
        public List<Category> ParentCategories { get; set; }
        public List<ProductInfo> RelatedProducts { get; set; }

        public ProductViewModel()
        {
            Product = new ProductInfo();
            Category = new Category();
            ParentCategories = new List<Category>();
            RelatedProducts = new List<ProductInfo>();
        }
    }
}