using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OnlineExam
{
    public class Logger
    {
        public static void WriteLogFile(string methodName, string message, string stacktrace)
        {

            try
            {
                string ErrorLogFilePath = string.Empty;
                string ErrorLogFolder = HttpContext.Current.Server.MapPath("~") + "ErrorLog";
                FileStream fs = null;

                //Creating Log Folder
                if (!System.IO.Directory.Exists(ErrorLogFolder))
                {
                    System.IO.Directory.CreateDirectory(ErrorLogFolder);
                }

                //Creating Log File Date wise
                ErrorLogFilePath = ErrorLogFolder + "\\Log@" + System.DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt";

                if (!File.Exists(ErrorLogFilePath))
                {
                    using (fs = File.Create(ErrorLogFilePath))
                    {
                    }
                }

                //Writing error message in the date wise created log file
                if (!string.IsNullOrEmpty(message))
                {
                    using (FileStream file = new FileStream(ErrorLogFilePath, FileMode.Append, FileAccess.Write))
                    {
                        StreamWriter streamWriter = new StreamWriter(file);
                        streamWriter.WriteLine(((((System.DateTime.Now + " - ") + methodName + " - ") + message + " - ") + stacktrace + "\r\n"));
                        streamWriter.WriteLine("=====================================================================");
                        streamWriter.WriteLine("\n");
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

    }
}