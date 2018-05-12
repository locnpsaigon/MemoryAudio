using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Home
{
    public class HomeViewModel
    {
        public List<Product> NewProducts { get; set; }
        public List<Product> HotProducts { get; set; }

        public HomeViewModel()
        {
            NewProducts = new List<Product>();
            HotProducts = new List<Product>();
        }
    }
}