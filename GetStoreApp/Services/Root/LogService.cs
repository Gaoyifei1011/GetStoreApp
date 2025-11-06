using GetStoreApp.Helpers.Root;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.System;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static readonly string notavailable = "N/A";
        private static readonly string httpRequestFolderPath = Path.Combine([ApplicationData.GetDefault().LocalCacheFolder.Path, "Logs", "HttpRequest"]);
        private static readonly string exceptionFolderPath = Path.Combine([ApplicationData.GetDefault().LocalCacheFolder.Path, "Logs", "Exception"]);
        private static readonly LoggingChannelOptions channelOptions = new();
        private static SemaphoreSlim logSemaphoreSlim = new(1, 1);

        public static string WinGetFolderPath { get; } = Path.Combine([ApplicationData.GetDefault().LocalCacheFolder.Path, "Logs", "WinGet"]);

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(LoggingLevel logLevel, string nameSpaceName, string className, string methodName, int index, Dictionary<string, string> loggingInformationDict)
        {
            Task.Run(async () =>
            {
                logSemaphoreSlim?.Wait();

                try
                {
                    if (!Directory.Exists(httpRequestFolderPath))
                    {
                        Directory.CreateDirectory(httpRequestFolderPath);
                    }

                    LoggingSession httpRequestSession = new("Http request log session");
                    LoggingChannel httpRequestChannel = new("Http request log channel", channelOptions);
                    LoggingFields httpRequestFields = new();
                    Guid httpRequestGuid = GuidHelper.CreateNewGuid();
                    LoggingOptions httpRequestOptions = new()
                    {
                        ActivityId = httpRequestGuid,
                        RelatedActivityId = httpRequestGuid
                    };

                    httpRequestSession.AddLoggingChannel(httpRequestChannel);
                    httpRequestFields.AddString("LogLevel", Convert.ToString(logLevel));
                    httpRequestFields.AddString("NameSpace", nameSpaceName);
                    httpRequestFields.AddString("Class", className);
                    httpRequestFields.AddString("Method", methodName);
                    httpRequestFields.AddString("Index", Convert.ToString(index));

                    foreach (KeyValuePair<string, string> loggingInformationItem in loggingInformationDict)
                    {
                        httpRequestFields.AddString(loggingInformationItem.Key, loggingInformationItem.Value);
                    }

                    string logFileName = string.Format("Logs-{0}-{1}-{2}-{3:D2}-{4}.etl", nameSpaceName, className, methodName, index, DateTimeOffset.Now.ToString("yyyy-MM-dd HH-mm-ss.fff"));
                    httpRequestChannel.LogEvent(logFileName, httpRequestFields, logLevel, httpRequestOptions);
                    await httpRequestSession.SaveToFileAsync(await Windows.Storage.StorageFolder.GetFolderFromPathAsync(httpRequestFolderPath), logFileName);
                    httpRequestSession.Dispose();
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    logSemaphoreSlim?.Release();
                }
            });
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(LoggingLevel logLevel, string nameSpaceName, string className, string methodName, int index, Exception exception)
        {
            Task.Run(async () =>
            {
                logSemaphoreSlim?.Wait();

                try
                {
                    if (!Directory.Exists(exceptionFolderPath))
                    {
                        Directory.CreateDirectory(exceptionFolderPath);
                    }

                    LoggingSession exceptionSession = new("Exception log session");
                    LoggingChannel exceptionChannel = new("Exception log channel", channelOptions);
                    LoggingFields exceptionFields = new();
                    Guid exceptionGuid = GuidHelper.CreateNewGuid();
                    LoggingOptions exceptionOptions = new()
                    {
                        ActivityId = exceptionGuid,
                        RelatedActivityId = exceptionGuid
                    };

                    exceptionSession.AddLoggingChannel(exceptionChannel);
                    exceptionFields.AddString("LogLevel", Convert.ToString(logLevel));
                    exceptionFields.AddString("NameSpace", nameSpaceName);
                    exceptionFields.AddString("Class", className);
                    exceptionFields.AddString("Method", methodName);
                    exceptionFields.AddString("Index", Convert.ToString(index));
                    exceptionFields.AddString("HelpLink", string.IsNullOrEmpty(exception.HelpLink) ? notavailable : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' '));
                    exceptionFields.AddString("Message", string.IsNullOrEmpty(exception.Message) ? notavailable : exception.Message.Replace('\r', ' ').Replace('\n', ' '));
                    exceptionFields.AddString("HResult", "0x" + Convert.ToString(exception.HResult, 16).ToUpper());
                    exceptionFields.AddString("Source", string.IsNullOrEmpty(exception.Source) ? notavailable : exception.Source.Replace('\r', ' ').Replace('\n', ' '));
                    exceptionFields.AddString("StackTrace", string.IsNullOrEmpty(exception.StackTrace) ? notavailable : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' '));

                    string logFileName = string.Format("Logs-{0}-{1}-{2}-{3:D2}-{4}.etl", nameSpaceName, className, methodName, index, DateTimeOffset.Now.ToString("yyyy-MM-dd HH-mm-ss.fff"));
                    exceptionChannel.LogEvent(logFileName, exceptionFields, logLevel, exceptionOptions);
                    await exceptionSession.SaveToFileAsync(await Windows.Storage.StorageFolder.GetFolderFromPathAsync(exceptionFolderPath), logFileName);
                    exceptionSession.Dispose();
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    logSemaphoreSlim?.Release();
                }
            });
        }

        /// <summary>
        /// 打开日志记录文件夹
        /// </summary>
        public static async Task OpenLogFolderAsync()
        {
            string logFolderPath = Path.Combine(ApplicationData.GetDefault().LocalCacheFolder.Path, "Logs");

            if (Directory.Exists(logFolderPath))
            {
                await Launcher.LaunchFolderPathAsync(logFolderPath);
            }
            else
            {
                await Launcher.LaunchFolderAsync(ApplicationData.GetDefault().LocalCacheFolder);
            }
        }

        /// <summary>
        /// 清除所有的日志文件
        /// </summary>
        public static async Task<bool> ClearLogAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    if (Directory.Exists(httpRequestFolderPath))
                    {
                        string[] httpRequestLogFiles = Directory.GetFiles(httpRequestFolderPath, "*.etl");

                        foreach (string httpRequestLogFile in httpRequestLogFiles)
                        {
                            DeleteFileHelper.DeleteFileToRecycleBin(httpRequestLogFile);
                        }
                    }

                    if (Directory.Exists(exceptionFolderPath))
                    {
                        string[] exceptionLogFiles = Directory.GetFiles(exceptionFolderPath, "*.etl");

                        foreach (string exceptionLogFile in exceptionLogFiles)
                        {
                            DeleteFileHelper.DeleteFileToRecycleBin(exceptionLogFile);
                        }
                    }
                });
                return true;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return false;
            }
        }

        /// <summary>
        /// 关闭日志记录服务
        /// </summary>
        public static void CloseLog()
        {
            logSemaphoreSlim.Dispose();
            logSemaphoreSlim = null;
        }
    }
}
