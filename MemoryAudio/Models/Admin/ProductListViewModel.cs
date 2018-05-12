using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Admin
{
    public class ProductListViewModel
    {
        public string FilterText { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int Display { get; set; }
        public string SortOrder { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IPagedList<ProductInfo> Products { get; set; }

        public List<SelectListItem> CategorySelector { get; set; }
        public List<SelectListItem> BrandSelector { get; set; }
        public List<SelectListItem> DisplaySelector { get; set; }

        public ProductListViewModel()
        {
            FilterText = "";
            CategoryId = 0;
            BrandId = 0;
            Display = 0;
            SortOrder = "";
            PageIndex = 1;
            PageSize = AppSettings.PAGE_SIZE;
            Products = new List<ProductInfo>().ToPagedList<ProductInfo>(1, AppSettings.PAGE_SIZE);

            CategorySelector = new List<SelectListItem>();
            BrandSelector = new List<SelectListItem>();
            DisplaySelector = new List<SelectListItem>();
        }
    }
}