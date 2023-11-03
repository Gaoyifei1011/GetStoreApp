using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static readonly object logLock = new object();

        private static bool IsInitialized = false;

        private static string unknown = "unknown";

        private static StorageFolder LogFolder;

        /// <summary>
        /// 初始化日志记录
        /// </summary>
        public static async Task InitializeAsync()
        {
            try
            {
                LogFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);
                IsInitialized = true;
            }
            catch (Exception)
            {
                IsInitialized = false;
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(LoggingLevel logLevel, string logContent, StringBuilder logBuilder)
        {
            if (IsInitialized)
            {
                try
                {
                    Task.Run(() =>
                    {
                        lock (logLock)
                        {
                            File.AppendAllText(
                                Path.Combine(LogFolder.Path, string.Format("GetStoreApp_{0}.log", DateTime.Now.ToString("yyyy_MM_dd"))),
                                string.Format("{0}\t{1}:{2}{3}{4}{5}{6}{7}{8}",
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    "LogLevel",
                                    Convert.ToString(logLevel),
                                    Environment.NewLine,
                                    "LogContent:",
                                    logContent,
                                    Environment.NewLine,
                                    logBuilder,
                                    Environment.NewLine)
                                );
                        }
                    });
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(LoggingLevel logLevel, string logContent, Exception exception)
        {
            if (IsInitialized)
            {
                try
                {
                    Task.Run(() =>
                    {
                        StringBuilder exceptionBuilder = new StringBuilder();
                        exceptionBuilder.Append("LogContent:");
                        exceptionBuilder.AppendLine(logContent);
                        exceptionBuilder.Append("HelpLink:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.HelpLink) ? unknown : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("Message:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.Message) ? unknown : exception.Message.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("HResult:");
                        exceptionBuilder.AppendLine(exception.HResult.ToString());
                        exceptionBuilder.Append("Source:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.Source) ? unknown : exception.Source.Replace('\r', ' ').Replace('\n', ' '));
                        exceptionBuilder.Append("StackTrace:");
                        exceptionBuilder.AppendLine(string.IsNullOrEmpty(exception.StackTrace) ? unknown : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' '));

                        lock (logLock)
                        {
                            File.AppendAllText(
                                Path.Combine(LogFolder.Path, string.Format("GetStoreApp_{0}.log", DateTime.Now.ToString("yyyy_MM_dd"))),
                                string.Format("{0}\t{1}:{2}{3}{4}{5}",
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    "LogLevel",
                                    Convert.ToString(logLevel),
                                    Environment.NewLine,
                                    exceptionBuilder.ToString(),
                                    Environment.NewLine)
                                );
                        }
                    });
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 打开日志记录文件夹
        /// </summary>
        public static async Task OpenLogFolderAsync()
        {
            if (IsInitialized)
            {
                await Launcher.LaunchFolderAsync(LogFolder);
            }
        }

        /// <summary>
        /// 清除所有的日志文件
        /// </summary>
        public static bool ClearLog()
        {
            try
            {
                Task.Run(() =>
                {
                    string[] logFiles = Directory.GetFiles(LogFolder.Path, "*.log");
                    foreach (string logFile in logFiles)
                    {
                        File.Delete(logFile);
                    }
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
