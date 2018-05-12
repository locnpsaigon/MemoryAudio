using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Admin
{
    public class BrandListViewModel
    {
        public string FilterText { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IPagedList<Brand> Brands { get; set; }

        public BrandListViewModel()
        {
            FilterText = "";
            PageIndex = 1;
            PageSize = AppSettings.PAGE_SIZE;
            Brands = new List<Brand>().ToPagedList<Brand>(1, AppSettings.PAGE_SIZE);
        }
    }
}