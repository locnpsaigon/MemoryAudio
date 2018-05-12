using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using PagedList;
using MemoryAudio.Libs;
using MemoryAudio.Models.Context;
using MemoryAudio.Models.Entities;
using MemoryAudio.Models.Search;

namespace MemoryAudio.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Results(int categoryId = 0, string searchText = "", string sortOrder = "", int page = 1, int pageSize = 12)
        {
            var model = new SearchResultsViewModel();
            try
            {
                model.SearchText = searchText.Trim();
                model.SortOrder = sortOrder;
                
                using(var db = new DBContext())
                {
                    var query = from p in db.Products
                                join c in db.Categories on p.CategoryId equals c.CategoryId into pc
                                join b in db.Brands on p.BrandId equals b.BrandId into pb
                                from j1 in pc.DefaultIfEmpty()
                                from j2 in pb.DefaultIfEmpty()
                                where p.Display > 0
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
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        query = query.Where(r => r.ProductName.Contains(searchText) || r.CategoryName.Contains(searchText));
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
                            query = query.OrderBy(p => p.ProductName);
                            break;
                    }
                    // Paging
                    var products = query.ToList();
                    var pageCount = (products.Count / pageSize) + (products.Count % pageSize > 0 ? 1 : 0);
                    if (page < pageCount)
                    {
                        page = pageCount;
                    }
                    model.Products = query.ToPagedList<ProductInfo>(page, pageSize);
                }
                model.PageIndex = model.Products.PageNumber;
                model.PageSize = model.Products.PageSize;
                return View(model);
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write(ex.ToString(), EventLogEntryType.Error);
                // Redirect to error page
                return RedirectToAction("Index", "Error");
            }
        }
    }
}