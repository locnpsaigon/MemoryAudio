using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using MemoryAudio.Models.Context;

namespace MemoryAudio.Models.Admin
{
    public class AddUserModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        [RegularExpression(@"^[a-z0-9_-]{2,15}$", ErrorMessage = "Tên tài khoản chỉ gồm chữ và số, gạch ngang và gạch dưới")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$", ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        [System.ComponentModel.DataAnnotations.Compare("PasswordConfirm", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Họ tên không được rỗng")]
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public List<SelectListItem> StatusSelector { get; set; }
        public List<RoleSelectorModel> RoleSelector { get; set; }

        public class RoleSelectorModel
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public Boolean Selected { get; set; } 
        }
    }
}