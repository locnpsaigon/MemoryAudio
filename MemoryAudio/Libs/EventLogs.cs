﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Configuration;
using System.Diagnostics;

namespace MemoryAudio.Libs
{
    public class EventLogs
    {
        public static void Write(string message, EventLogEntryType type = EventLogEntryType.Error)
        {
            // Write logs to file
            try
            {
                var filePath = HostingEnvironment.MapPath(
                    AppSettings.LOG_PATH + DateTime.Now.Year +
                    ((DateTime.Now.Month < 10) ? ("0" + DateTime.Now.Month) : DateTime.Now.Month.ToString()) +
                    ((DateTime.Now.Day < 10) ? ("0" + DateTime.Now.Day) : DateTime.Now.Day.ToString()) + "_logs.txt");
                if (!File.Exists(filePath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                        sw.WriteLine("---------------------------------------");
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        sw.WriteLine(message);
                        sw.WriteLine("---------------------------------------");
                    }
                }
                else
                {
                    // This text is always added, making the file longer over time
                    // if it is not deleted.
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine("---------------------------------------");
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        sw.WriteLine(message);
                        sw.WriteLine("---------------------------------------");
                    }
                }
            }
            catch { }

            // Write event logs
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "MemoryAudio";
                    eventLog.WriteEntry(message, type, 1080, 1);
                }
            }
            catch { }
        }
    }
}