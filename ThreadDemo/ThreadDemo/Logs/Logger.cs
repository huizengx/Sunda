using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ThreadDemo.Logs
{
    public enum LogType
    {
        Info,
        Warning,
        Error
    }

    class Logger
    {
        private static Queue<string> _queueMsg = new Queue<string>();
        private static bool _IsExit = false;
        private static string _logFilePath;

        static Logger()
        {
            _logFilePath = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".log";
            var task = new Task(LoggerMessage, TaskCreationOptions.LongRunning);
            task.Start();
        }

        private static void LoggerMessage()
        {
            while (!_IsExit)
            {
                if (_queueMsg.Count!=0)
                {
                    var msg = _queueMsg.Dequeue();
                    using (StreamWriter writer = new StreamWriter(_logFilePath,true, Encoding.UTF8))
                    {
                        writer.WriteLine(msg);
                        writer.Close();
                    }
                }
                //System.Threading.Thread.Yield();
            }
        }

        public static void WriteLog(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                _queueMsg.Enqueue(msg);
            }
        }

    }
}
