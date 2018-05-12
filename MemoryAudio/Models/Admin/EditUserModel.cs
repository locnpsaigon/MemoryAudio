using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class EditUserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
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