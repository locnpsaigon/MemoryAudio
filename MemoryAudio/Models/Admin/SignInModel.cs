using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class SignInModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        public string Password { get; set; }

        public Boolean Remember { get; set; }
    }
}