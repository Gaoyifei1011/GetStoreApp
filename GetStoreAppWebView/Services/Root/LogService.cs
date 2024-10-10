using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
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
        private static readonly string unknown = "unknown";
        private static readonly string exceptionFolderPath = Path.Combine([ApplicationData.Current.LocalCacheFolder.Path, "Logs", "Exception"]);
        private static readonly LoggingChannelOptions channelOptions = new();
        private static SemaphoreSlim logSemaphoreSlim = new(1, 1);

        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(LoggingLevel logLevel, string logContent, Dictionary<string, string> loggingInformationDict)
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
                    Guid exceptionGuid = Guid.NewGuid();
                    LoggingOptions exceptionOptions = new()
                    {
                        ActivityId = exceptionGuid,
                        RelatedActivityId = exceptionGuid
                    };

                    exceptionSession.AddLoggingChannel(exceptionChannel);
                    exceptionFields.AddString("LogLevel", logLevel.ToString());

                    foreach (KeyValuePair<string, string> logInformationKeyValueItem in loggingInformationDict)
                    {
                        exceptionFields.AddString(logInformationKeyValueItem.Key, logInformationKeyValueItem.Value);
                    }

                    exceptionChannel.LogEvent(logContent, exceptionFields, logLevel, exceptionOptions);
                    await exceptionSession.SaveToFileAsync(await StorageFolder.GetFolderFromPathAsync(exceptionFolderPath), string.Format("Logs {0} {1}.etl", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), exceptionGuid.ToString().ToUpper()));
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
        public static void WriteLog(LoggingLevel logLevel, string logContent, Exception exception)
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
                    Guid exceptionGuid = Guid.NewGuid();
                    LoggingOptions exceptionOptions = new()
                    {
                        ActivityId = exceptionGuid,
                        RelatedActivityId = exceptionGuid
                    };

                    exceptionSession.AddLoggingChannel(exceptionChannel);
                    exceptionFields.AddString("LogLevel", logLevel.ToString());
                    exceptionFields.AddString("HelpLink", string.IsNullOrEmpty(exception.HelpLink) ? unknown : exception.HelpLink.Replace('\r', ' ').Replace('\n', ' '));
                    exceptionFields.AddString("Message", string.IsNullOrEmpty(exception.Message) ? unknown : exception.Message.Replace('\r', ' ').Replace('\n', ' '));
                    exceptionFields.AddString("HResult", exception.HResult.ToString());
                    exceptionFields.AddString("Source", string.IsNullOrEmpty(exception.Source) ? unknown : exception.Source.Replace('\r', ' ').Replace('\n', ' '));
                    exceptionFields.AddString("StackTrace", string.IsNullOrEmpty(exception.StackTrace) ? unknown : exception.StackTrace.Replace('\r', ' ').Replace('\n', ' '));

                    exceptionChannel.LogEvent(logContent, exceptionFields, logLevel, exceptionOptions);
                    await exceptionSession.SaveToFileAsync(await StorageFolder.GetFolderFromPathAsync(exceptionFolderPath), string.Format("Logs {0} {1}.etl", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), exceptionGuid.ToString().ToUpperInvariant()));
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
