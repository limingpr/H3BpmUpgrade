using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H3BpmUpgrade.Helper
{
    /// <summary>
    /// NLog日志记录辅助类
    /// </summary>
    public class LogHelper
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        public static void Debug(object ex)
        {
            Log.Debug(ex);
        }

        public static void Warn(object ex)
        {
            Log.Warn(ex);
        }

        public static void Error(object ex)
        {
            Log.Error(ex);
        }

        public static void Info(object ex)
        {
            Log.Info(ex);
        }


        public static void Trace(object ex)
        {
            Log.Trace(ex);
        }

        public static void Fatal(object ex)
        {
            Log.Fatal(ex);
        }
    }
}
