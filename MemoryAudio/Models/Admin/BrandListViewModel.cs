using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Admin
{
    public class UserListViewModel
    {
        public string FilterText { get; set; }
        public int Status { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IPagedList<User> Users { get; set; }

        public UserListViewModel()
        {
            FilterText = "";
            Status = 0;
            PageIndex = 1;
            PageSize = AppSettings.PAGE_SIZE;
            Users = new List<User>().ToPagedList<User>(1, AppSettings.PAGE_SIZE);
        }
    }
}