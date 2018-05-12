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
    public class NewsViewModel
    {
        public News News { get; set; }
        public List<News> RelatedNews { get; set; }

        public NewsViewModel()
        {
            News = new News();
            RelatedNews = new List<News>();
        }
    }
}