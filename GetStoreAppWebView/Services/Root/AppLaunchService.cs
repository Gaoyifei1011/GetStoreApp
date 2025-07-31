using GetStoreAppWebView.Extensions.DataType.Classes;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreAppWebView.Services.Root
{
    /// <summary>
    /// 应用启动参数服务
    /// </summary>
    public static class AppLaunchService
    {
        private const string appLaunch = "AppLaunch";
        private const string parameter = "Parameter";

        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer appLaunchContainer;

        /// <summary>
        /// 初始化应用启动记录存储服务
        /// </summary>
        public static void Initialize()
        {
            appLaunchContainer = localSettingsContainer.CreateContainer(appLaunch, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 保存应用启动参数
        /// </summary>
        public static async Task SaveArgumentsAsync(AppLaunchArguments appLaunchArguments)
        {
            await Task.Run(() =>
            {
                try
                {
                    appLaunchContainer.Values[nameof(appLaunchArguments.AppLaunchKind)] = Convert.ToString(appLaunchArguments.AppLaunchKind);
                    appLaunchContainer.Values[nameof(appLaunchArguments.IsLaunched)] = appLaunchArguments.IsLaunched;

                    if (appLaunchArguments.SubParameters is not null)
                    {
                        ApplicationDataContainer parameterContainer = appLaunchContainer.CreateContainer(parameter, ApplicationDataCreateDisposition.Always);
                        for (int index = 0; index < appLaunchArguments.SubParameters.Count; index++)
                        {
                            parameterContainer.Values[string.Format("Data{0}", index + 1)] = appLaunchArguments.SubParameters[index];
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppWebView), nameof(AppLaunchService), nameof(SaveArgumentsAsync), 1, e);
                }
            });
        }
    }
}
