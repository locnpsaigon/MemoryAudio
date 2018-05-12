using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? Status { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual ICollection<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
        }

        public Boolean IsInRole(int roleId)
        {
            foreach (var role in Roles)
            {
                if (role.RoleId == roleId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}