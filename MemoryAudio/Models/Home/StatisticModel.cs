using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemoryAudio.Models.Context;
using MemoryAudio.Models.Entities;

namespace MemoryAudio.Models.Home
{
    public class StatisticModel
    {
        
        public static Statistic GetCurrentStatistic()
        {
            try
            {
                using(var db = new DBContext())
                {
                    var today = DateTime.Now;
                    var dateFrom = new DateTime(today.Year, today.Month, today.Day);
                    var dateTo = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                    var statistic = db.Statistics.Where(r => r.Date >= dateFrom && r.Date <= dateTo).FirstOrDefault();
                    if (statistic == null)
                    {
                        statistic = new Statistic();
                        statistic.Visitors = 1;
                        statistic.Online = 1;
                        db.Statistics.Add(statistic);
                        db.SaveChanges();
                    }
                    return statistic;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static Statistic GetYesterdayStatistic()
        {
            try
            {
                using (var db = new DBContext())
                {
                    var yesterday = DateTime.Now.AddDays(- 1);
                    var dateFrom = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day);
                    var dateTo = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                    var statistic = db.Statistics.Where(r => r.Date >= dateFrom && r.Date <= dateTo).FirstOrDefault();
                    if (statistic == null)
                    {
                        statistic = new Statistic();
                        statistic.Visitors = db.Statistics.AsEnumerable().Sum(r => r.Visitors);
                        statistic.Online = 0;
                    }
                    return statistic;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static int GetTotalVisitors()
        {
            try
            {
                using (var db = new DBContext())
                {
                    return db.Statistics.AsEnumerable().Sum(r => r.Visitors);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static void IncreaseOnline()
        {
            try
            {
                using (var db = new DBContext())
                {
                    var today = DateTime.Now;
                    var dateFrom = new DateTime(today.Year, today.Month, today.Day);
                    var dateTo = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                    var statistic = db.Statistics.Where(r => r.Date >= dateFrom && r.Date <= dateTo).FirstOrDefault();
                    if (statistic == null)
                    {
                        statistic = new Statistic();
                        statistic.Visitors = 1;
                        statistic.Online = 1;
                        db.Statistics.Add(statistic);
                    }
                    else
                    {
                        statistic.Visitors = statistic.Visitors + 1;
                        statistic.Online = statistic.Online + 1;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static void DecreaseOnline()
        {
            try
            {
                using (var db = new DBContext())
                {
                    var today = DateTime.Now;
                    var dateFrom = new DateTime(today.Year, today.Month, today.Day);
                    var dateTo = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                    var statistic = db.Statistics.Where(r => r.Date >= dateFrom && r.Date <= dateTo).FirstOrDefault();
                    if (statistic == null)
                    {
                        statistic = new Statistic();
                        statistic.Visitors = 1;
                        statistic.Online = 0;
                        db.Statistics.Add(statistic);
                    }
                    else
                    {
                        statistic.Online = statistic.Online > 0 ? statistic.Online - 1 : 0;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}