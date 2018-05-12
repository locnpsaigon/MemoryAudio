using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemoryAudio.Models.Security;

namespace MemoryAudio.Controllers
{
    public class BaseController : Controller
    {
        protected virtual new MAPrinciple User
        {
            get { return HttpContext.User as MAPrinciple; }
        }

        protected string GetRequestedIP()
        {
            return Request.ServerVariables["REMOTE_ADDR"];
        }

        protected string GetLogonUserName()
        {
            return User == null ? "Guest" : User.Identity.Name;
        }
    }
}