using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [Compare("NewPasswordConfirm", ErrorMessage = "Mật khẩu và mật khấu xác nhận phải trùng khớp")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$", ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public string NewPassword { get; set; }

        public string NewPasswordConfirm { get; set; }
    }
}