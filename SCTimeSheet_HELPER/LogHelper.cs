using SCTimeSheet_UTIL;
using System;
using System.IO;
using SCTimeSheet_DAL;
using SCTimeSheet_DAL.Models;

namespace SCTimeSheet_HELPER
{
    public class LogHelper
    {
        #region Private Variables

        private static readonly string LogPath = ReadConfig.GetValue("LogPath");
        private static readonly object lockObj = new object();

        #endregion

        #region Error Log
        public static void ErrorLog(Exception exception)
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    ErrorLogModel log = new ErrorLogModel
                    {
                        Message = exception.Message + "\n" + exception.GetBaseException(),
                        StackTrace = exception.StackTrace,
                        Datetime = DateTime.UtcNow
                    };
                    db.ErrorLog.Add(log);
                    db.SaveChanges();
                }
            }
            catch
            {
                try
                {
                    WriteErrorToFile(exception);
                }
                catch
                {
                   //No need
                }
            }
        }

        public static ErrorLogModel LogError(Exception exception, Boolean returnLog)
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    ErrorLogModel log = new ErrorLogModel
                    {
                        Message = exception.Message + "\n" + exception.GetBaseException(),
                        StackTrace = exception.StackTrace,
                        Datetime = DateTime.UtcNow
                    };
                    db.ErrorLog.Add(log);
                    db.SaveChanges();
                    return log;
                }
            }
            catch
            {
                try
                {
                    WriteErrorToFile(exception);
                }
                catch
                {
                    //No need
                }
            }

            return null;
        }

        public static void WriteErrorToFile(Exception exception)
        {
            try
            {
                string message = exception.Message;
                if (exception.InnerException != null)
                {
                    message += " --base: " + exception.GetBaseException().Message;
                }

                lock (lockObj)
                {
                    try
                    {
                        string directory = LogPath;
                        string fileName = LogPath + @"/ErrorLog_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
                        TextWriter writer;

                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        writer = File.AppendText(fileName);
                        writer.WriteLine(string.Format(DateTime.Now.ToString("HH:mm:ss - {0}"), message));
                        writer.Close();
                    }
                    catch { }
                }
            }
            catch { }
        }

        #endregion

        //#region Activity Log
        //public static void ActivityLog(string message)
        //{
        //    try
        //    {
        //        using (var db = new ApplicationDBContext())
        //        {
        //            ActivityLogModel log = new ActivityLogModel
        //            {
        //                Activity = message,
        //                Datetime = DateTime.UtcNow
        //            };
        //            db.ActivityLog.Add(log);
        //            db.SaveChanges();
        //        }
        //    }
        //    catch
        //    {
        //        try
        //        {
        //            WriteActivityToFile(message);
        //        }
        //        catch
        //        {
        //            //unable to write to file, check permission!
        //            //this shouldn't happen
        //        }
        //    }
        //}

        //public static void WriteActivityToFile(string message)
        //{
        //    try
        //    {
        //        lock (lockObj)
        //        {
        //            try
        //            {
        //                string directory = LogPath;
        //                string fileName = LogPath + @"/ActivityLog_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
        //                TextWriter writer;

        //                if (!Directory.Exists(directory))
        //                    Directory.CreateDirectory(directory);

        //                writer = File.AppendText(fileName);
        //                writer.WriteLine(string.Format(DateTime.Now.ToString("HH:mm:ss - {0}"), message));
        //                writer.Close();
        //            }
        //            catch { }
        //        }
        //    }
        //    catch { }
        //}

        //#endregion
    }
}
