using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;

namespace GetStoreAppWebView.Services.Root
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public static class LogService
    {
        private static readonly string notavailable = "N/A";
        private static readonly string exceptionFolderPath = Path.Combine([ApplicationData.GetDefault().LocalCacheFolder.Path, "Logs", "Exception"]);
        private static readonly LoggingChannelOptions channelOptions = new();
        private static SemaphoreSlim logSemaphoreSlim = new(1, 1);

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

                    foreach (KeyValuePair<string, string> logInformationItem in loggingInformationDict)
                    {
                        exceptionFields.AddString(logInformationItem.Key, logInformationItem.Value);
                    }

                    string logFileName = string.Format("Logs-{0}-{1}-{2}-{3:D2}-{4}.etl", nameSpaceName, className, methodName, index, DateTimeOffset.Now.ToString("yyyy-MM-dd HH-mm-ss.fff"));
                    exceptionChannel.LogEvent(logFileName, exceptionFields, logLevel, exceptionOptions);
                    await exceptionSession.SaveToFileAsync(await Windows.Storage.StorageFolder.GetFolderFromPathAsync(exceptionFolderPath), logFileName);
                    exceptionSession.Dispose();
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    return;
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
                    exceptionFields.AddString("HelpLink", string.IsNullOrEmpty(exception.HelpLink) ? notavailable : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' '));
                    exceptionFields.AddString("Message", string.IsNullOrEmpty(exception.Message) ? notavailable : exception.Message.Replace('\r', ' ').Replace('\n', ' '));
                    exceptionFields.AddString("HResult", "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant());
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
                    return;
                }
                finally
                {
                    logSemaphoreSlim?.Release();
                }
            });
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
