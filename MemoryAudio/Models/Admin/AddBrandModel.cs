using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class AddBrandModel
    {
        [Required(ErrorMessage = "Tênthương hiệu không được rỗng")]
        public string BrandName { get; set; }
        public string Description { get; set; }

        public AddBrandModel()
        {
            BrandName = "";
            Description = "";
        }
    }
}