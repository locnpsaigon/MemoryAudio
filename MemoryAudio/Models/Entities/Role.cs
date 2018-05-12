using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Role()
        {
            Users = new List<User>();
        }
    }
}