using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Utilities
{
    public class ExceptionLogHelp
    {
        public static void WriteLog(Exception ex)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string _watherFilePath = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            if (!File.Exists(_watherFilePath))
            {
                File.Create(_watherFilePath).Close();
            }

            File.AppendAllText(_watherFilePath, string.Format("****{0}****\r\n###{1}\r\n###{2}\r\n###{3}\r\n###{4}\r\n", System.DateTime.Now.ToString(), ex.Message, ex.Source, ex.TargetSite, ex.StackTrace));
        }
        public static void WriteLog(string exception)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string _watherFilePath = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            if (!File.Exists(_watherFilePath))
            {
                File.Create(_watherFilePath).Close();
            }

            File.AppendAllText(_watherFilePath, string.Format("****{0}****\r\n###{1}\r\n", System.DateTime.Now.ToString(), exception));
        }
    }
}