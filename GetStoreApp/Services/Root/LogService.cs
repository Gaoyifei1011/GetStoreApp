using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static partial class LogService
    {
        private static char lineBreak = '\n';

        private static string unknown = "unknown";

        [GeneratedRegex("[\r\n]")]
        private static partial Regex WhiteSpace();

        public static Regex WhiteSpaceRegex { get; } = WhiteSpace();

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
        public static void WriteLog(LoggingLevel logType, string logContent, StringBuilder logBuilder)
        {
            if (IsInitialized)
            {
                try
                {
                    Task.Run(() =>
                    {
                        File.AppendAllText(
                            Path.Combine(LogFolder.Path, string.Format("GetStoreApp_{0}.log", DateTime.Now.ToString("yyyy_MM_dd"))),
                            string.Format("{0}\t{1}\t{2}\n{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToString(logType), logContent, logBuilder)
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
        public static void WriteLog(LoggingLevel logType, string logContent, Exception exception)
        {
            if (IsInitialized)
            {
                try
                {
                    Task.Run(() =>
                    {
                        StringBuilder exceptionBuilder = new StringBuilder();
                        exceptionBuilder.Append("\t\tHelpLink:");
                        exceptionBuilder.Append(string.IsNullOrEmpty(exception.HelpLink) ? unknown : WhiteSpaceRegex.Replace(exception.HelpLink, " "));
                        exceptionBuilder.Append(lineBreak);
                        exceptionBuilder.Append("\t\tMessage:");
                        exceptionBuilder.Append(string.IsNullOrEmpty(exception.Message) ? unknown : WhiteSpaceRegex.Replace(exception.Message, " "));
                        exceptionBuilder.Append(lineBreak);
                        exceptionBuilder.Append("\t\tHResult:");
                        exceptionBuilder.Append(exception.HResult);
                        exceptionBuilder.Append(lineBreak);
                        exceptionBuilder.Append("\t\tSource:");
                        exceptionBuilder.Append(string.IsNullOrEmpty(exception.Source) ? unknown : WhiteSpaceRegex.Replace(exception.Source, " "));
                        exceptionBuilder.Append(lineBreak);
                        exceptionBuilder.Append("\t\tStackTrace:");
                        exceptionBuilder.Append(string.IsNullOrEmpty(exception.StackTrace) ? unknown : WhiteSpaceRegex.Replace(exception.StackTrace, ""));
                        exceptionBuilder.Append(lineBreak);

                        File.AppendAllText(
                            Path.Combine(LogFolder.Path, string.Format("GetStoreApp_{0}.log", DateTime.Now.ToString("yyyy_MM_dd"))),
                            string.Format("{0}\t{1}\t{2}\n{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToString(logType), logContent, exceptionBuilder.ToString())
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
