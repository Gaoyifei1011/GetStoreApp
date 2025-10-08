using GetStoreApp.Services.Root;
using System;
using Windows.Foundation.Diagnostics;
using Windows.Networking.Connectivity;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 网络状态检测辅助类
    /// </summary>
    public static class NetWorkHelper
    {
        /// <summary>
        /// 检测网络是否已经连接
        /// </summary>
        public static bool IsNetWorkConnected()
        {
            try
            {
                ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                if (connectionProfile is not null)
                {
                    NetworkConnectivityLevel networkConnectivityLevel = connectionProfile.GetNetworkConnectivityLevel();
                    return networkConnectivityLevel is NetworkConnectivityLevel.LocalAccess || networkConnectivityLevel is NetworkConnectivityLevel.ConstrainedInternetAccess || networkConnectivityLevel is NetworkConnectivityLevel.InternetAccess;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(NetWorkHelper), nameof(IsNetWorkConnected), 1, e);
                return false;
            }
        }
    }
}
