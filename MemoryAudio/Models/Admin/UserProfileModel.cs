using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class UserProfileModel
    {
        [Required(ErrorMessage = "Họ tên không được rỗng")]
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public List<string> Roles { get; set; }

        public UserProfileModel()
        {
            Roles = new List<string>();
        }
    }
}