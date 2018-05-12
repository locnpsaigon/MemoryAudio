using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Admin
{
    public class RoleListViewModel
    {
        public string FilterText { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IPagedList<Role> Roles { get; set; }

        public RoleListViewModel()
        {
            FilterText = "";
            PageIndex = 1;
            PageSize = AppSettings.PAGE_SIZE;
            Roles = new List<Role>().ToPagedList<Role>(1, AppSettings.PAGE_SIZE);
        }
    }
}