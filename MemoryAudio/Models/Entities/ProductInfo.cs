﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Entities
{
    public class ProductInfo
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public string Specification { get; set; }
        public int TotalInStock { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal MSRP { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }
        public string Image6 { get; set; }
        public DateTime CreationDate { get; set; }
        public int Display { get; set; }
        public string DisplayName { get; set; }
        public int SortIdx { get; set; }

        /// <summary>
        /// Constructors
        /// </summary>
        public ProductInfo()
        {
            ProductId = 0;
            ProductName = "";
            CategoryId = 0;
            CategoryName = "";
            BrandId = 0;
            BrandName = "";
            Specification = "";
            TotalInStock = 0;
            Price = 0;
            Discount = 0;
            Display = 0;
            DisplayName = "";
            Image1 = "";
            Image2 = "";
            Image3 = "";
            Image4 = "";
            Image5 = "";
            Image6 = "";
            CreationDate = DateTime.MinValue;
            SortIdx = int.MaxValue;
        }


        
    }
}