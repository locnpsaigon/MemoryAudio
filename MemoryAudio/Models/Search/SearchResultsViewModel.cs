using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Search
{
    public class SearchResultsViewModel
    {
        public string SearchText { get; set; }
        public string SortOrder { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public Category Category { get; set; }
        public IPagedList<ProductInfo> Products { get; set; }
    }
}