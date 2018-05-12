using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class EditBrandModel
    {
        [Key]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Tênthương hiệu không được rỗng")]
        public string BrandName { get; set; }

        public string Description { get; set; }

        public EditBrandModel()
        {
            BrandId = 0;
            BrandName = "";
            Description = "";
        }
    }
}