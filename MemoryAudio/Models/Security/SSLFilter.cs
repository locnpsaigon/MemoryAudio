using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MemoryAudio.Models.Security
{
    public class SSLFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsSecureConnection)
            {
                var url = filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:");
                filterContext.Result = new RedirectResult(url);
            }
        }
    }
}