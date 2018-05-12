using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class AddNewsModel
    {
        [Required(ErrorMessage = "Tiêu đề không được rỗng")]
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Lead { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại tin")]
        public int Type { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn trạng thái hiển thị")]
        public int Status { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày xuất bản")]
        public string ReleaseDate { get; set; }
        public List<SelectListItem> TypeSelector { get; set; }
        public List<SelectListItem> StatusSelector { get; set; }

        public AddNewsModel()
        {
            Title = "";
            Icon = "";
            Lead = "";
            Body = "";
            Tags = "";
            TypeSelector = new List<SelectListItem>();
            StatusSelector = new List<SelectListItem>();
        }
    }
}