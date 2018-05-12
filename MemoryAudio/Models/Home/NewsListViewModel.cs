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
    public class NewsListViewModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IPagedList<News> News { get; set; }

        public NewsListViewModel()
        {
            PageIndex = 1;
            PageSize = AppSettings.PAGE_SIZE;
            News = new List<News>().ToPagedList(1, PageSize);
        }
    }
}