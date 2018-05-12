using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace MemoryAudio.Models.Security
{
    public class MAPrinciple : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public int UserId { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }

        public MAPrinciple(string username)
        {
            this.Identity = new GenericIdentity(username);
            this.Roles = new List<string>();
        }

        public Boolean IsInRole(string roleName)
        {
            foreach(var item in Roles)
            {
                if (string.Compare(roleName, item) == 0)
                {
                    return true;
                }
            }   
            return false;
        }
    }
}