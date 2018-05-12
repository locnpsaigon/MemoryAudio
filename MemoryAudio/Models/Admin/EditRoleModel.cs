using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Admin
{
    public class EditRoleModel
    {
        [Key]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "Tên chức danh không được rỗng")]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}