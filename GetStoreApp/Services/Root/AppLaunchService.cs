using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.Windows.Storage;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用启动参数服务
    /// </summary>
    public static class AppLaunchService
    {
        private const string appLaunch = "AppLaunch";
        private const string parameter = "Parameter";

        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
        private static ApplicationDataContainer appLaunchContainer;

        /// <summary>
        /// 初始化应用启动记录存储服务
        /// </summary>
        public static void Initialize()
        {
            appLaunchContainer = localSettingsContainer.CreateContainer(appLaunch, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 读取应用启动参数
        /// </summary>
        public static async Task<AppLaunchArguments> ReadArgumentsAsync()
        {
            AppLaunchArguments appLaunchArguments = new();

            await Task.Run(() =>
            {
                try
                {
                    appLaunchArguments.AppLaunchKind = Enum.TryParse(Convert.ToString(appLaunchContainer.Values[nameof(appLaunchArguments.AppLaunchKind)]), out AppLaunchKind appLaunchKind) ? appLaunchKind : AppLaunchKind.None;
                    appLaunchArguments.IsLaunched = Convert.ToBoolean(appLaunchContainer.Values[nameof(appLaunchArguments.IsLaunched)]);

                    if (appLaunchContainer.Containers.TryGetValue(parameter, out ApplicationDataContainer parameterContainer))
                    {
                        appLaunchArguments.SubParameters = [];
                        for (int index = 0; index < parameterContainer.Values.Count; index++)
                        {
                            if (parameterContainer.Values.TryGetValue(string.Format("Data{0}", index + 1), out object parameterobj) && parameterobj is string parameter)
                            {
                                appLaunchArguments.SubParameters.Add(parameter);
                            }
                        }
                    }

                    appLaunchContainer.DeleteContainer(parameter);
                    appLaunchContainer.Values.Clear();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppLaunchService), nameof(ReadArgumentsAsync), 1, e);
                }
            });

            return appLaunchArguments;
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppLaunchService), nameof(SaveArgumentsAsync), 1, e);
                }
            });
        }
    }
}
