using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Newtonsoft.Json;
using MemoryAudio.Models.Security;
using MemoryAudio.Models.Home;

namespace MemoryAudio
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AppSettings.LoadSettings();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Update statistic
            StatisticModel.IncreaseOnline();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Update statistic
            StatisticModel.DecreaseOnline();
        }

        protected void Application_PostAuthenticateRequest()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                var authUserData = JsonConvert.DeserializeObject<AuthUserData>(authTicket.UserData);
                var principle = new MAPrinciple(authTicket.Name);
                principle.UserId = authUserData.UserId;
                principle.FullName = authUserData.FullName;
                principle.Roles = authUserData.Roles;
                HttpContext.Current.User = principle;
            }
        }
    }
}
