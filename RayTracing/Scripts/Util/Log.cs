using System.Collections.Concurrent;

namespace RayTracing
{
    public class Log
    {
        private static object threadRestartLockObj = new object();

        private const int SLEEP_TIME = 250;

        private class LogItem
        {
            public string Message;
            public object[] Parameters;

            public bool WriteToFile;
            public bool WriteToConsole;
            public bool WriteToNewLine;

            // if it's not info, it's automatically debug. (Atleast currently).
            public bool IsInfo;

        }

        private static ConcurrentQueue<LogItem> itemsToLog;
        private static Thread logThread;

        public static bool FileWriteEnabled { get; set; }

        public static bool ConsoleEnabled { get; set; }

        public static string LogFilePath { get; set; }

        public static bool InfoEnabled { get; set; }

        public static bool DebugEnabled { get; set; }

        public static void Initialise(String logFilePath)
        {
            Initialise(logFilePath, true, true, true, true);
        }

        public static void Initialise(String logFilePath, bool consoleEnabled, bool fileWriteEnabled, bool infoEnabled, bool debugEnabled)
        {
            LogFilePath = logFilePath;
            ConsoleEnabled = consoleEnabled;
            FileWriteEnabled = fileWriteEnabled;
            InfoEnabled = infoEnabled;
            DebugEnabled = debugEnabled;

            itemsToLog = new ConcurrentQueue<LogItem>();
        }

        private static void LogWorkerThread()
        {
            while (itemsToLog.Count > 0)
            {
                LogItem item;
                if (itemsToLog.TryPeek(out item))
                {
                    if (TryProcessItem(item))
                    {
                        while (!itemsToLog.TryDequeue(out item))
                            Thread.Sleep(SLEEP_TIME);
                    }
                }
                else
                    Thread.Sleep(SLEEP_TIME);
            }
        }

        private static void CheckLogThread()
        {
            lock (threadRestartLockObj)
            {
                if (logThread == null || !logThread.IsAlive)
                {
                    logThread = new Thread(LogWorkerThread);
                    logThread.Start();
                }
            }
        }

        private static bool TryProcessItem(LogItem item)
        {
            try
            {
                if (item.WriteToFile)
                    WriteToLogFile(item.Message, item.WriteToNewLine, item.Parameters);
            }
            catch
            {
                return false;
            }

            if (item.WriteToConsole)
                WriteToConsole(item.Message, item.WriteToNewLine, item.Parameters);

            return true;
        }

        public static void Debug(String str, params object[] param)
        {
            DebugLog(str, false, param);
        }

        public static void DebugLine(String str, params object[] param)
        {
            DebugLog(str, true, param);
        }

        private static void DebugLog(string str, bool writeToNewLine, params object[] param)
        {
            if (DebugEnabled)
            {
                str = String.Format("[{0} Debug]: {1}", DateTime.Now, str);

                itemsToLog.Enqueue(new LogItem()
                {
                    IsInfo = false,
                    Message = str,
                    Parameters = param,
                    WriteToConsole = ConsoleEnabled,
                    WriteToFile = FileWriteEnabled,
                    WriteToNewLine = writeToNewLine
                });

                CheckLogThread();
            }
        }

        public static void Info(String str, params object[] param)
        {
            InfoLog(str, false, param);
        }

        public static void InfoLine(String str, params object[] param)
        {
            InfoLog(str, true, param);
        }

        private static void InfoLog(string str, bool writeToNewLine, params object[] param)
        {
            if (InfoEnabled)
            {
                str = String.Format("[{0} Info]: {1}", DateTime.Now, str);

                itemsToLog.Enqueue(new LogItem()
                {
                    IsInfo = true,
                    Message = str,
                    Parameters = param,
                    WriteToConsole = ConsoleEnabled,
                    WriteToFile = FileWriteEnabled,
                    WriteToNewLine = writeToNewLine
                });

                CheckLogThread();
            }
        }

        private static void WriteToConsole(String str, bool writeToNewLine, params object[] param)
        {
            if (writeToNewLine)
                Console.WriteLine();

            Console.Write(str, param);
        }

        private static void WriteToLogFile(String str, bool writeToNewLine, params object[] param)
        {
            if (!File.Exists(LogFilePath))
                File.Create(LogFilePath);

            using (FileStream fs = File.Open(LogFilePath, FileMode.Append))
            {
                TextWriter textWriter = new StreamWriter(fs);

                if (writeToNewLine)
                    textWriter.WriteLine();

                textWriter.Write(string.Format(str, param));
                textWriter.Close();
            }
        }
    }
}