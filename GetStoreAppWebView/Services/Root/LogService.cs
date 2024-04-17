using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreAppWebView.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static bool isInitialized = false;

        private static string unknown = "unknown";
        private static string logFolderPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Logs");

        /// <summary>
        /// 初始化日志记录
        /// </summary>
        public static void Initialize()
        {
            try
            {
                if (!Directory.Exists(Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Logs")))
                {
                    Directory.CreateDirectory(Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Logs"));
                }
                isInitialized = true;
            }
            catch (Exception)
            {
                isInitialized = false;
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(EventLogEntryType logType, StringBuilder logBuilder)
        {
            if (isInitialized)
            {
                try
                {
                    Task.Run(() =>
                    {
                        File.AppendAllText(
                            Path.Combine(logFolderPath, string.Format("GetStoreApp_{0}.log", DateTime.Now.ToString("yyyy_MM_dd"))),
                            string.Format("{0}\t{1}:{2}\n{3}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "LogLevel", Convert.ToString(logType), logBuilder)
                            );
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
        public static void WriteLog(EventLogEntryType logType, string logContent, Exception exception)
        {
            if (isInitialized)
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

                        File.AppendAllText(
                            Path.Combine(logFolderPath, string.Format("GetStoreApp_{0}.log", DateTime.Now.ToString("yyyy_MM_dd"))),
                            string.Format("{0}\t{1}:{2}\n{3}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "LogLevel", Convert.ToString(logType), exceptionBuilder.ToString())
                            );
                    });
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
