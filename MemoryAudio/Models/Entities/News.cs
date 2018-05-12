using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemoryAudio.Models.Entities
{
    /// <summary>
    /// Lớp đại diện bài viết tin tức
    /// </summary>
    public class News
    {
        public int NewsId { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Lead { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Constructors
        /// </summary>
        public News()
        {
            NewsId = 0;
            Title = "";
            Icon = "";
            Lead = "";
            Body = "";
            Type = 0;
            Status = 0;
            CreationDate = DateTime.Now;
        }
    }
}