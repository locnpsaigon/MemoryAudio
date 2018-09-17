using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using MemoryAudio.Models;
using PagedList;
using MemoryAudio.Models.Admin.CategoryTree;
using MemoryAudio.Models.Context;
using MemoryAudio.Models.Entities;
using MemoryAudio.Models.Home;
using MemoryAudio.Libs;

namespace MemoryAudio.Controllers
{
    public class HomeController : Controller
    {
        static int ParseId(string source)
        {
            var pattern = @"\d+$";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                return int.Parse(match.Groups[0].Value);
            }
            return 0;
        }

        public ActionResult Index()
        {
            try
            {
                // Set meta data
                ViewBag.MetaDescription = "Trang chủ Memory Audio";
                ViewBag.MetaKeywords = "memory-audio,am-thanh-chau-au,audiophile,sound,hifi,stereo,hi-end,hd,ultra-hd,dts,dts-hd";

                var model = new HomeViewModel();
                using (var db = new DBContext())
                {
                    model.NewProducts = db.Products.Where(r => r.Display == 3).OrderByDescending(r => r.SortIdx).Take(AppSettings.PAGE_SIZE).ToList();
                    model.HotProducts = db.Products.Where(r => r.Display == 4).OrderByDescending(r => r.SortIdx).Take(AppSettings.PAGE_SIZE).ToList();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write("HomeController - Index: " + ex.ToString(), EventLogEntryType.Error);

                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Category(String name, string filterText = "", string sortOrder = "", int page = 1, int pageSize = 36)
        {
            var model = new CategoryViewModel();
            try
            {
                model.FilterText = filterText.Trim();
                model.SortOrder = sortOrder;
                using (var db = new DBContext())
                {
                    // Get category id
                    var categoryId = ParseId(name);

                    // Get category info
                    var category = db.Categories.Where(r => r.CategoryId == categoryId).FirstOrDefault();
                    if (category != null)
                    {
                        // Set meta data
                        ViewBag.MetaDescription = "Memory Audio - " + category.CategoryName;
                        ViewBag.MetaKeywords = category.CategoryName + ",memory-audio,audiophile,sound,hifi,stereo,hi-end,hd,ultra-hd,dts,dts-hd";

                        // Query products
                        var query = from p in db.Products
                                    join c in db.Categories on p.CategoryId equals c.CategoryId into pc
                                    join b in db.Brands on p.BrandId equals b.BrandId into pb
                                    from j1 in pc.DefaultIfEmpty()
                                    from j2 in pb.DefaultIfEmpty()
                                    where p.Display > 1
                                    select new ProductInfo
                                    {
                                        ProductId = p.ProductId,
                                        ProductName = p.ProductName,
                                        CategoryId = p.CategoryId,
                                        CategoryName = j1.CategoryName,
                                        BrandId = p.BrandId,
                                        BrandName = j2.BrandName,
                                        Specification = p.Specification,
                                        TotalInStock = p.TotalInStock,
                                        Price = p.Price,
                                        Discount = p.Discount,
                                        Image1 = p.Image1,
                                        Image2 = p.Image2,
                                        Image3 = p.Image3,
                                        Image4 = p.Image4,
                                        Image5 = p.Image5,
                                        Image6 = p.Image6,
                                        CreationDate = p.CreationDate,
                                        Display = p.Display,
                                        SortIdx = p.SortIdx
                                    };

                        // Get category nodes include its children and parent
                        var categoryNode = new CategoryTreeNode();
                        categoryNode.CategoryId = category.CategoryId;
                        categoryNode.CategoryName = category.CategoryName;
                        categoryNode.Description = category.Description;
                        categoryNode.Level = 0;
                        categoryNode.Parent = null;
                        categoryNode.Nodes = new List<CategoryTreeNode>();
                        CategoryTree.AppendChildNodes(categoryNode);
                        CategoryTree.AppendParentNodes(categoryNode);

                        // Filter products by category which included its children 
                        var childNodes = categoryNode.GetChildNodes();
                        var childCategoryIds = new List<int>();
                        foreach (var node in childNodes)
                        {
                            childCategoryIds.Add(node.CategoryId);
                        }
                        if (childCategoryIds.Count > 0)
                        {
                            query = query.Where(r => childCategoryIds.Contains(r.CategoryId ?? 0));
                        }
                        if (!string.IsNullOrWhiteSpace(filterText))
                        {
                            query = query.Where(r => r.ProductName.Contains(filterText) || r.CategoryName.Contains(filterText));
                        }

                        // Get parent nodes
                        var parentNodes = categoryNode.GetParentNodes();
                        foreach (var parentNode in parentNodes)
                        {
                            var parentCategory = db.Categories.Where(r => r.CategoryId == parentNode.CategoryId).FirstOrDefault();
                            if (parentCategory != null && parentCategory.CategoryId != category.CategoryId)
                            {
                                model.ParentCategories.Add(parentCategory);
                            }
                        }

                        // Sorting
                        switch (sortOrder)
                        {
                            case "price":
                                query = query.OrderBy(p => p.Price);
                                break;

                            case "price_desc":
                                query = query.OrderByDescending(p => p.Price);
                                break;

                            case "name":
                                query = query.OrderBy(p => p.ProductName);
                                break;

                            case "name_desc":
                                query = query.OrderByDescending(p => p.ProductName);
                                break;

                            default:
                                query = query.OrderByDescending(p => p.SortIdx);
                                break;
                        }
                        var products = query.ToList();
                        var pageCount = (products.Count / pageSize) + (products.Count % pageSize > 0 ? 1 : 0);
                        if (page > pageCount)
                        {
                            page = pageCount;
                        }
                        model.Products = query.ToPagedList<ProductInfo>(page == 0 ? 1 : page, pageSize);
                    }
                    else
                    {
                        model.Products = new List<ProductInfo>().ToPagedList(1, pageSize);
                    }

                    model.Category = category;
                    model.PageIndex = model.Products.PageNumber;
                    model.PageSize = model.Products.PageSize;

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write("HomeController - Category: " + ex.ToString(), EventLogEntryType.Error);

                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult NewProducts(string filterText = "", string sortOrder = "", int page = 1, int pageSize = 36)
        {
            var model = new NewProductsViewModel();
            try
            {
                // Set meta data
                ViewBag.MetaDescription = "Memory Audio - Sản phẩm mới";
                ViewBag.MetaKeywords = "memory audio,new products,san pham moi,audiophile,sound,hifi,stereo,hi-end,hd,ultra-hd,dts,dts-hd";

                using (var db = new DBContext())
                {
                    model.FilterText = filterText.Trim();
                    model.SortOrder = sortOrder;
                    // Select products
                    var query = from p in db.Products
                                join c in db.Categories on p.CategoryId equals c.CategoryId into pc
                                join b in db.Brands on p.BrandId equals b.BrandId into pb
                                from j1 in pc.DefaultIfEmpty()
                                from j2 in pb.DefaultIfEmpty()
                                where p.Display == 3
                                select new ProductInfo
                                {
                                    ProductId = p.ProductId,
                                    ProductName = p.ProductName,
                                    CategoryId = p.CategoryId,
                                    CategoryName = j1.CategoryName,
                                    BrandId = p.BrandId,
                                    BrandName = j2.BrandName,
                                    Specification = p.Specification,
                                    TotalInStock = p.TotalInStock,
                                    Price = p.Price,
                                    Discount = p.Discount,
                                    Image1 = p.Image1,
                                    Image2 = p.Image2,
                                    Image3 = p.Image3,
                                    Image4 = p.Image4,
                                    Image5 = p.Image5,
                                    Image6 = p.Image6,
                                    CreationDate = p.CreationDate,
                                    Display = p.Display,
                                    SortIdx = p.SortIdx
                                };

                    // Filter
                    if (!string.IsNullOrWhiteSpace(filterText))
                    {
                        query = query.Where(r => r.ProductName.Contains(filterText) || r.CategoryName.Contains(filterText));
                    }

                    // Sorting
                    switch (sortOrder)
                    {
                        case "price":
                            query = query.OrderBy(p => p.Price);
                            break;

                        case "price_desc":
                            query = query.OrderByDescending(p => p.Price);
                            break;

                        case "name":
                            query = query.OrderBy(p => p.ProductName);
                            break;

                        case "name_desc":
                            query = query.OrderByDescending(p => p.ProductName);
                            break;

                        default:
                            query = query.OrderByDescending(p => p.SortIdx);
                            break;
                    }
                    var products = query.ToList();
                    var pageCount = (products.Count() / pageSize) + (products.Count() % pageSize > 0 ? 1 : 0);
                    if (page > pageCount)
                    {
                        page = pageCount;
                    }
                    model.Products = query.ToPagedList<ProductInfo>(page == 0 ? 1 : page, pageSize);
                    
                }
                model.PageIndex = model.Products.PageNumber;
                model.PageSize = model.Products.PageSize;
                return View(model);
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write("HomeController - NewProducts: " + ex.ToString(), EventLogEntryType.Error);

                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult HotProducts(string filterText = "", string sortOrder = "", int page = 1, int pageSize = 36)
        {
            var model = new HotProductsViewModel();
            try
            {
                // Set meta data
                ViewBag.MetaDescription = "Memory Audio - Sản phẩm nổi bật";
                ViewBag.MetaKeywords = "memory audio,hot products,san pham noi bat,audiophile,sound,hifi,stereo,hi-end,hd,ultra-hd,dts,dts-hd";

                using (var db = new DBContext())
                {
                    model.FilterText = filterText.Trim();
                    model.SortOrder = sortOrder;
                    // Select products
                    var query = from p in db.Products
                                join c in db.Categories on p.CategoryId equals c.CategoryId into pc
                                join b in db.Brands on p.BrandId equals b.BrandId into pb
                                from j1 in pc.DefaultIfEmpty()
                                from j2 in pb.DefaultIfEmpty()
                                where p.Display == 4
                                select new ProductInfo
                                {
                                    ProductId = p.ProductId,
                                    ProductName = p.ProductName,
                                    CategoryId = p.CategoryId,
                                    CategoryName = j1.CategoryName,
                                    BrandId = p.BrandId,
                                    BrandName = j2.BrandName,
                                    Specification = p.Specification,
                                    TotalInStock = p.TotalInStock,
                                    Price = p.Price,
                                    Discount = p.Discount,
                                    Image1 = p.Image1,
                                    Image2 = p.Image2,
                                    Image3 = p.Image3,
                                    Image4 = p.Image4,
                                    Image5 = p.Image5,
                                    Image6 = p.Image6,
                                    CreationDate = p.CreationDate,
                                    Display = p.Display,
                                    SortIdx = p.SortIdx
                                };

                    // Filter
                    if (!string.IsNullOrWhiteSpace(filterText))
                    {
                        query = query.Where(r => r.ProductName.Contains(filterText) || r.CategoryName.Contains(filterText));
                    }

                    // Sorting
                    switch (sortOrder)
                    {
                        case "price":
                            query = query.OrderBy(p => p.Price);
                            break;

                        case "price_desc":
                            query = query.OrderByDescending(p => p.Price);
                            break;

                        case "name":
                            query = query.OrderBy(p => p.ProductName);
                            break;

                        case "name_desc":
                            query = query.OrderByDescending(p => p.ProductName);
                            break;

                        default:
                            query = query.OrderByDescending(p => p.SortIdx);
                            break;
                    }
                    var products = query.ToList();
                    var pageCount = (products.Count() / pageSize) + (products.Count() % pageSize > 0 ? 1 : 0);
                    if (page > pageCount)
                    {
                        page = pageCount;
                    }
                    model.Products = query.ToPagedList<ProductInfo>(page == 0 ? 1 : page, pageSize);
                }
                model.PageIndex = model.Products.PageNumber;
                model.PageSize = model.Products.PageSize;
                return View(model);
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write("HomeController - HotProducts: " + ex.ToString(), EventLogEntryType.Error);

                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Product(string name)
        {
            var model = new ProductViewModel();
            try
            {
                using (var db = new DBContext())
                {
                    // Get ProductId
                    var productId = ParseId(name);

                    // Get product info
                    var productInfo = (from p in db.Products
                                       join c in db.Categories on p.CategoryId equals c.CategoryId into pc
                                       join b in db.Brands on p.BrandId equals b.BrandId into pb
                                       from j1 in pc.DefaultIfEmpty()
                                       from j2 in pb.DefaultIfEmpty()
                                       where p.ProductId == productId && p.Display > 1
                                       select new ProductInfo
                                       {
                                           ProductId = p.ProductId,
                                           ProductName = p.ProductName,
                                           CategoryId = p.CategoryId,
                                           CategoryName = j1.CategoryName,
                                           BrandId = p.BrandId,
                                           BrandName = j2.BrandName,
                                           Specification = p.Specification,
                                           TotalInStock = p.TotalInStock,
                                           Price = p.Price,
                                           Discount = p.Discount,
                                           MSRP = p.MSRP,
                                           Image1 = p.Image1,
                                           Image2 = p.Image2,
                                           Image3 = p.Image3,
                                           Image4 = p.Image4,
                                           Image5 = p.Image5,
                                           Image6 = p.Image6,
                                           CreationDate = p.CreationDate,
                                           Display = p.Display,
                                           SortIdx = p.SortIdx
                                       }).FirstOrDefault();

                    // Product existed?
                    if (productInfo == null)
                    {
                        return RedirectToAction("Index", "Error", new { @message = "Product not found!" });
                    }

                    // Get related products
                    var relatedProducts = from p in db.Products
                                          join c in db.Categories on p.CategoryId equals c.CategoryId into pc
                                          join b in db.Brands on p.BrandId equals b.BrandId into pb
                                          from j1 in pc.DefaultIfEmpty()
                                          from j2 in pb.DefaultIfEmpty()
                                          where p.Display > 1 && p.ProductId != productInfo.ProductId && p.CategoryId == productInfo.CategoryId
                                          orderby p.Display descending, p.SortIdx ascending
                                          select new ProductInfo
                                          {
                                              ProductId = p.ProductId,
                                              ProductName = p.ProductName,
                                              CategoryId = p.CategoryId,
                                              CategoryName = j1.CategoryName,
                                              BrandId = p.BrandId,
                                              BrandName = j2.BrandName,
                                              Specification = p.Specification,
                                              TotalInStock = p.TotalInStock,
                                              Price = p.Price,
                                              Discount = p.Discount,
                                              Image1 = p.Image1,
                                              Image2 = p.Image2,
                                              Image3 = p.Image3,
                                              Image4 = p.Image4,
                                              Image5 = p.Image5,
                                              Image6 = p.Image6,
                                              CreationDate = p.CreationDate,
                                              Display = p.Display,
                                              SortIdx = p.SortIdx
                                          };

                    // Get category info and its parent
                    var category = db.Categories.Where(r => r.CategoryId == productInfo.CategoryId).FirstOrDefault();
                    if (category != null)
                    {
                        var categoryNode = new CategoryTreeNode();
                        categoryNode.CategoryId = category.CategoryId;
                        categoryNode.CategoryName = category.CategoryName;
                        categoryNode.Description = category.Description;
                        categoryNode.Level = 0;
                        categoryNode.Parent = null;
                        categoryNode.Nodes = new List<CategoryTreeNode>();
                        CategoryTree.AppendParentNodes(categoryNode);

                        var parentNodes = categoryNode.GetParentNodes();
                        foreach (var parentNode in parentNodes)
                        {
                            var parentCategory = db.Categories.Where(r => r.CategoryId == parentNode.CategoryId).FirstOrDefault();
                            if (parentCategory != null && parentCategory.CategoryId != category.CategoryId)
                            {
                                model.ParentCategories.Add(parentCategory);
                            }
                        }
                        model.Category = category;
                    }
                    model.Product = productInfo;
                    model.RelatedProducts = relatedProducts.Take(AppSettings.PAGE_SIZE).ToList();

                    // Set meta data
                    ViewBag.MetaDescription = "Memory Audio - " + productInfo.ProductName;
                    ViewBag.MetaKeywords = 
                        productInfo.CategoryName + "," + 
                        productInfo.ProductName + "," + 
                        productInfo.BrandName + ",memory audio,audiophile,sound,hifi,stereo,hi-end,hd,ultra-hd,dts,dts-hd";

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write("HomeController - Products: " + ex.ToString(), EventLogEntryType.Error);

                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Promotion(int page = 1, int pageSize = 36)
        {
            try
            {
                using (var db = new DBContext())
                {
                    // Get promotion news
                    var promotions = db.News.Where(r => r.Type == 2).OrderByDescending(r => r.ReleaseDate);
                    // Paging
                    var pageCount = (promotions.Count() / pageSize) + (promotions.Count() % pageSize > 0 ? 1 : 0);
                    if (page > pageCount)
                    {
                        page = pageCount;
                    }
                    var model = new NewsListViewModel();
                    model.News = promotions.ToList().ToPagedList<News>(page == 0 ? 1 : page, pageSize);
                    model.PageIndex = page;
                    model.PageSize = pageSize;

                    // Set meta data
                    ViewBag.MetaDescription = "Memory Audio - Thông tin khuyến mãi";
                    ViewBag.MetaKeywords = "memory audio,promotion,khuyen mai,audiophile,sound,hifi,stereo,hi-end,hd,ultra-hd,dts,dts-hd";

                    return View(model);
                }
            }
            catch(Exception ex)
            {
                // Write event logs
                EventLogs.Write("HomeController - Promotion: " + ex.ToString(), EventLogEntryType.Error);

                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Reviews(int page = 1, int pageSize = 36)
        {
            try
            {
                using (var db = new DBContext())
                {
                    // Get promotion news
                    var promotions = db.News.Where(r => r.Type == 1).OrderByDescending(r => r.ReleaseDate);
                    // Paging
                    var pageCount = (promotions.Count() / pageSize) + (promotions.Count() % pageSize > 0 ? 1 : 0);
                    if (page > pageCount)
                    {
                        page = pageCount;
                    }
                    var model = new NewsListViewModel();
                    model.News = promotions.ToList().ToPagedList<News>(page == 0 ? 1 : page, pageSize);
                    model.PageIndex = page;
                    model.PageSize = pageSize;

                    // Set meta data
                    ViewBag.MetaDescription = "Memory Audio - Đánh giá sản phẩm";
                    ViewBag.MetaKeywords = "memory audio,review,danh gia,audiophile,sound,hifi,stereo,hi-end,hd,ultra-hd,dts,dts-hd";

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write("HomeController - Promotion: " + ex.ToString(), EventLogEntryType.Error);

                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult News(string title)
        {
            try
            {
                using (var db = new DBContext())
                {
                    // Get newsId
                    var newsId = ParseId(title);

                    // Get news
                    var news = db.News.Where(r => r.NewsId == newsId && r.Status > 1).OrderByDescending(r => r.ReleaseDate).FirstOrDefault();
                    if (news == null)
                    {
                        throw new Exception("Có lỗi xảy ra! Bản tin không tồn tại.");
                    }
                    // Get related news
                    var relatedNews = db.News
                        .Where(r => r.NewsId < news.NewsId && r.Type == news.Type && r.Status > 1)
                        .OrderByDescending(r => r.ReleaseDate)
                        .Take(AppSettings.PAGE_SIZE)
                        .ToList();
                    // Create view model
                    var model = new NewsViewModel();
                    model.News = news;
                    model.RelatedNews = relatedNews;

                    // Set meta data
                    ViewBag.MetaDescription = "Memory Audio - " + news.Title;
                    ViewBag.MetaKeywords = news.Tags;

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write("HomeController - News: " + ex.ToString(), EventLogEntryType.Error);

                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Payment()
        {
            ViewBag.Message = "Your application description page.";

            // Set meta data
            ViewBag.MetaDescription = "Memory Audio - Thông tin thanh toán";
            ViewBag.MetaKeywords = "payment,thanh toan";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            // Set meta data
            ViewBag.MetaDescription = "Memory Audio - Về chúng tôi";
            ViewBag.MetaKeywords = "about,gioi thieu,intro";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            // Set meta data
            ViewBag.MetaDescription = "Memory Audio - Thông tin liên hệ";
            ViewBag.MetaKeywords = "contact,about,gioi thieu,intro";
            return View();
        }

        public ActionResult Sitemap()
        {
            return View();
        }
    }
}