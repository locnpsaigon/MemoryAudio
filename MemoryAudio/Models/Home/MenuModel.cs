using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using MemoryAudio.Models.Entities;
using MemoryAudio.Models.Context;
using MemoryAudio.Libs;

namespace MemoryAudio.Models.Home
{
    public class MenuModel
    {
        static int LOADING_TIMEOUT = 300;   // caching for 5 minutes 
        static DateTime LastTopMenuUpdated = DateTime.MinValue;
        static DateTime LastSideMenuUpdated = DateTime.MinValue;

        static string _topMenuRawHtml = "";
        static string _sideMenuRawHtml = "";

        /// <summary>
        /// Get top menu raw html
        /// </summary>
        /// <returns></returns>
        public static string GetTopMenuItems()
        {
            try
            {
                var isTimedOut = (DateTime.Now - LastTopMenuUpdated).TotalSeconds > LOADING_TIMEOUT;
                if (isTimedOut)
                {
                    using (var db = new DBContext())
                    {
                        var categories = db.Categories.Where(r => r.ParentId == null).OrderBy(r => r.SortIdx).ToList();
                        var html = "";
                        html += "<ul class=\"right\" style=\"margin-bottom:0px;\">";
                        html += "    <li><a href=\"\\Trang-chu\">Trang chủ</a></li>";
                        html += "    <li><a href=\"\\Gioi-thieu\">Giới thiệu</a></li>";
                        html += "    <li><a href=\"#\">Sản phẩm</a>" + AppendChildMenuItems(categories) + "</li>";
                        html += "    <li><a href=\"\\Khuyen-mai\">Khuyến mãi</a></li>";
                        html += "    <li><a href=\"\\Danh-gia\">Đánh giá</a></li>";
                        html += "    <li><a href=\"\\Thanh-toan\">Thanh toán</a></li>";
                        html += "    <li><a href=\"\\Lien-he\">Liên hệ</a></li>";
                        html += "</ul>";
                        _topMenuRawHtml = html;
                        LastTopMenuUpdated = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write(ex.ToString(), EventLogEntryType.Error);
            }
            return _topMenuRawHtml;
        }

        /// <summary>
        /// Get side menu raw html
        /// </summary>
        /// <returns></returns>
        public static string GetSideMenuItems()
        {

            var isTimedOut = (DateTime.Now - LastSideMenuUpdated).TotalSeconds > LOADING_TIMEOUT;
            if (isTimedOut)
            {
                _sideMenuRawHtml = "";

                using (var db = new DBContext())
                {
                    var categories = db.Categories.Where(r => r.ParentId == null).OrderBy(r => r.SortIdx).ToList();
                    _sideMenuRawHtml = AppendChildMenuItems(categories);
                }

                LastSideMenuUpdated = DateTime.Now;
            }
            return _sideMenuRawHtml;
        }

        /// <summary>
        /// Popuplate category menu tree
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        private static string AppendChildMenuItems(List<Category> categories)
        {
            var htmlMenu = "<ul>";
            foreach (var category in categories)
            {
                using (var db = new DBContext())
                {
                    var sub_categories = db.Categories.Where(r => r.ParentId == category.CategoryId).OrderBy(r => r.SortIdx).ToList();
                    if (sub_categories.Count > 0)
                    {
                        //htmlMenu += "<li><a href=\"/Home/Category?categoryId=" + category.CategoryId + "\">" + category.CategoryName + "</a>" + AppendChildMenuItems(sub_categories) + "</li>";
                        htmlMenu += "<li><a>" + category.CategoryName + "</a>" + AppendChildMenuItems(sub_categories) + "</li>";
                    }
                    else
                    {
                        var categoryName = category.CategoryName.ToHyperLinkTitle() + "-" + category.CategoryId;
                        htmlMenu += "<li><a href=\"/Phan-loai/" + categoryName + "\">" + category.CategoryName + "</a></li>";
                    }
                }
            }
            htmlMenu += "</ul>";
            return htmlMenu;
        }
    }
}