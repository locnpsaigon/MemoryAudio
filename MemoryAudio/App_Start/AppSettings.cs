using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Configuration;
using MemoryAudio.Libs;

namespace MemoryAudio
{
    public class AppSettings
    {
        public static int PAGE_SIZE = 12;
        public static string LOG_PATH = @"~\App_Data\Logs\";

        public static void LoadSettings()
        {
            try
            {
                LOG_PATH = ConfigurationManager.AppSettings["LOG_PATH"];
                PAGE_SIZE = Int32.Parse(ConfigurationManager.AppSettings["PAGE_SIZE"]);
            }
            catch (Exception ex)
            {
                // Write event logs
                EventLogs.Write(ex.ToString(), EventLogEntryType.Error);
            }
        }
    }
}