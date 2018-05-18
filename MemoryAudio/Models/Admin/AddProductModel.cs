using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class AddProductModel
    {
        [Required(ErrorMessage = "Tên sản phẩm không được rỗng")]
        public string ProductName { get; set; }

        public string Specification { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public int CategoryId { get; set; }
        public List<SelectListItem> CategorySelector { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thương hiệu sản phẩm")]
        public int BrandId { get; set; }
        public List<SelectListItem> BrandSelector { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tồn")]
        public string TotalInStock { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public string Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giảm giá")]
        public string Discount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá niêm yết")]
        public string MSRP { get; set; }

        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }
        public string Image6 { get; set; }

        public int Display { get; set; }    // 0: Hidden, 1: Normal, 2:New: 3:Hot
        public List<SelectListItem> DisplaySelector { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thứ tự sáp xếp")]
        [RegularExpression("[0-9]*", ErrorMessage = "Thứ tự sắp xếp không hợp lệ")]
        public string SortIdx { get; set; }

        public AddProductModel()
        {
            CategorySelector = new List<SelectListItem>();
            BrandSelector = new List<SelectListItem>();
            DisplaySelector = new List<SelectListItem>();
        }
    }
}