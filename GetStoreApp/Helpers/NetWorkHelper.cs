using GetStoreApp.Extensions.Enum;
using Windows.Networking.Connectivity;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// 网络状态监控服务
    /// </summary>
    public static class NetWorkHelper
    {
        public static NetWorkStatus GetNetWorkStatus()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();

            // 获取与本地计算机当前使用的 Internet 连接关联的连接配置文件
            if (connectionProfile is null)
            {
                return NetWorkStatus.None;
            }

            // 定义当前可用的连接级别
            NetworkConnectivityLevel networkConnectivityLevel = connectionProfile.GetNetworkConnectivityLevel();

            if (networkConnectivityLevel == NetworkConnectivityLevel.None)
            {
                return NetWorkStatus.None;
            }

            // 获取一个值，该值指示连接配置文件是否为 WWAN (移动) 连接
            if (connectionProfile.IsWwanConnectionProfile)
            {
                if (connectionProfile.WwanConnectionProfileDetails == null)
                {
                    return NetWorkStatus.Unknown;
                }
                else
                {
                    WwanDataClass connectionStatus = connectionProfile.WwanConnectionProfileDetails.GetCurrentDataClass();

                    switch (connectionStatus)
                    {
                        // 2G网络状态
                        case WwanDataClass.Gprs: return NetWorkStatus.IIG;
                        // 3G网络状态
                        case WwanDataClass.Hsupa: return NetWorkStatus.IIIG;
                        // 4G网络状态
                        case WwanDataClass.LteAdvanced: return NetWorkStatus.IVG;
                        // 不明确的网络状态
                        case WwanDataClass.None: return NetWorkStatus.Unknown;
                        default: return NetWorkStatus.Unknown;
                    }
                }
            }
            // 获取一个值，该值指示连接配置文件是否为 WLAN (WiFi) 连接。
            else if (connectionProfile.IsWlanConnectionProfile)
            {
                return NetWorkStatus.WIFI;
            }
            // 不是无线网（WIFI）也不是移动网络则判断为LAN网络
            else
            {
                return NetWorkStatus.LAN;
            }
        }
    }
}
