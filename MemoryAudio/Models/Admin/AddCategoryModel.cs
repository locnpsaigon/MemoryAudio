﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class AddCategoryModel
    {
        [Required(ErrorMessage = "Tên danh mục không được rỗng")]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public int SortIdx { get; set; }
        public List<SelectListItem> ParentSelecor { get; set; }

        public AddCategoryModel()
        {
            CategoryName = "";
            Description = "";
            ParentId = 0;
            SortIdx = 1000;
            ParentSelecor = new List<SelectListItem>();
        }
    }
}