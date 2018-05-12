using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MemoryAudio.Models.Security
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected virtual MAPrinciple CurrentUser
        {
            get
            {
                return HttpContext.Current.User as MAPrinciple;
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext != null)
            {
                if (CurrentUser != null || CurrentUser.Identity.IsAuthenticated)
                {
                    if (CurrentUser.IsInRole(Roles) || Users.Contains(CurrentUser.Identity.Name))
                    {
                        SetCachePolicy(filterContext);
                    }
                    else
                    {
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                }
                else
                {
                    // Unauthenticated, redirect to access denied page
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Error",
                                action = "Unauthorized",
                                message = "You have reached to an access denied resource!!!"
                            }));
                }
            }
            else
            {
                throw new ArgumentNullException("Invalid filterContext argument!!!");
            }
            base.OnAuthorization(filterContext);
        }

        public void CacheValidationHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        protected void SetCachePolicy(AuthorizationContext filterContext)
        {
            // ** IMPORTANT **
            // Since we're performing authorization at the action level, the authorization code runs
            // after the output caching module. In the worst case this could allow an authorized user
            // to cause the page to be cached, then an unauthorized user would later be served the
            // cached page. We work around this by telling proxies not to cache the sensitive page,
            // then we hook our custom authorization code into the caching mechanism so that we have
            // the final say on whether a page should be served from the cache.
            HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
            cachePolicy.SetProxyMaxAge(new TimeSpan(0));
            cachePolicy.AddValidationCallback(CacheValidationHandler, null /* data */);
        }

    }
}