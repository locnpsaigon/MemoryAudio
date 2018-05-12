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
    public class NewsListViewModel
    {
        public string FilterText { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IPagedList<News> News { get; set; }
        public List<SelectListItem> TypeSelector { get; set; }
        public List<SelectListItem> StatusSelector { get; set; }

        public NewsListViewModel()
        {
            FilterText = "";
            Type = 0;
            Status = 0;
            PageIndex = 1;
            PageSize = AppSettings.PAGE_SIZE;
            News = new List<News>().ToPagedList<News>(1, AppSettings.PAGE_SIZE);
        }
    }
}