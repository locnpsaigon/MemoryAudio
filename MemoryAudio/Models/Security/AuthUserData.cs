using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Security
{
    public class AuthUserData
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }

        public AuthUserData()
        {
            Roles = new List<string>();
        }
    }
}