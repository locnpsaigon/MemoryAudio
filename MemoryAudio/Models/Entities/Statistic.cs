using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MemoryAudio.Models.Entities
{
    /// <summary>
    /// Lớp đại diện thống kê online users
    /// </summary>
    public class Statistic
    {
        [Key]
        public DateTime Date { get; set; }
        public int Visitors { get; set; }
        public int Online { get; set; }

        public Statistic()
        {
            var today = DateTime.Now;
            Date =  new DateTime(today.Year, today.Month, today.Day);
            Visitors = 0;
            Online = 0;
        }
    }
}