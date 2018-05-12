using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using MemoryAudio.Models.Entities;
using MemoryAudio.Models.Admin.CategoryTree;

namespace MemoryAudio.Models.Home
{
    public class CategoryViewModel
    {
        public string FilterText { get; set; }
        public string SortOrder { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public Category Category { get; set; }
        public List<Category> ParentCategories { get; set; }
        public IPagedList<ProductInfo> Products { get; set; }

        public CategoryViewModel()
        {
            FilterText = "";
            SortOrder = "";
            PageIndex = 1;
            PageSize = AppSettings.PAGE_SIZE;
            Category = new Category();
            ParentCategories = new List<Category>();
            Products = new List<ProductInfo>().ToPagedList<ProductInfo>(1, AppSettings.PAGE_SIZE);
        }
    }
}