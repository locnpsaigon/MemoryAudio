using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Home
{
    public class HotProductsViewModel
    {
        public string FilterText { get; set; }
        public string SortOrder { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IPagedList<ProductInfo> Products { get; set; }

        public HotProductsViewModel()
        {
            FilterText = "";
            SortOrder = "";
            PageIndex = 1;
            PageSize = AppSettings.PAGE_SIZE;
            Products = new List<ProductInfo>().ToPagedList<ProductInfo>(1, AppSettings.PAGE_SIZE);
        }
    }
}