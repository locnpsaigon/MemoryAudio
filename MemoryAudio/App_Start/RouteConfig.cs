using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MemoryAudio
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Home
            routes.MapRoute(
                "Home",
                "Trang-chu",
                new { controller = "Home", action = "Index" }
            );

            // About
            routes.MapRoute(
                "About",
                "Gioi-thieu",
                new { controller = "Home", action = "About" }
            );

            // Contact
            routes.MapRoute(
                "Contact",
                "Lien-he",
                new { controller = "Home", action = "Contact" }
            );

            // Payment
            routes.MapRoute(
                "Payment",
                "Thanh-toan",
                new { controller = "Home", action = "Payment" }
            );

            // Promotion
            routes.MapRoute(
                "Promotion",
                "Khuyen-mai",
                new { controller = "Home", action = "Promotion" }
            );
            routes.MapRoute(
                "PromotionDetails",
                "Khuyen-mai/{title}",
                new { controller = "Home", action = "News" }
            );

            // Reviews
            routes.MapRoute(
                "Reviews",
                "Danh-gia",
                new { controller = "Home", action = "Reviews" }
            );
            routes.MapRoute(
                "ReviewsDetails",
                "Danh-gia/{title}",
                new { controller = "Home", action = "News" }
            );

            // Products
            routes.MapRoute(
                "Product",
                "San-pham/{name}",
                new { controller = "Home", action = "Product" }
            );
            routes.MapRoute(
                "NewProducts",
                "San-pham-moi",
                new { controller = "Home", action = "NewProducts" }
            );
            routes.MapRoute(
               "HotProducts",
               "San-pham-noi-bat",
               new { controller = "Home", action = "HotProducts" }
           );

            // Category
            routes.MapRoute(
               "Category",
               "Phan-loai/{name}",
               new { controller = "Home", action = "Category" }
           );

            // Default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
