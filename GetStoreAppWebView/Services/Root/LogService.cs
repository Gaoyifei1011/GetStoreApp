using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreAppWebView.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static string unknown = "unknown";

        private static bool IsInitialized { get; set; } = false;

        private static StorageFolder LogFolder { get; set; }

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
                        File.AppendAllText(
                            Path.Combine(LogFolder.Path, string.Format("GetStoreApp_{0}.log", DateTime.Now.ToString("yyyy_MM_dd"))),
                            string.Format("{0}\t{1}:{2}\n{3}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "LogLevel", Convert.ToString(logLevel), logBuilder)
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

                        File.AppendAllText(
                            Path.Combine(LogFolder.Path, string.Format("GetStoreApp_{0}.log", DateTime.Now.ToString("yyyy_MM_dd"))),
                            string.Format("{0}\t{1}:{2}\n{3}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "LogLevel", Convert.ToString(logLevel), exceptionBuilder.ToString())
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
