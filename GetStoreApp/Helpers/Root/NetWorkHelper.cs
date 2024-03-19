using Windows.Networking.Connectivity;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 网络状态监控辅助类
    /// </summary>
    public static class NetWorkHelper
    {
        public static bool IsNetworkConnected(out bool checkFailed)
        {
            try
            {
                ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                bool netWorkResult = connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                checkFailed = false;
                return netWorkResult;
            }
            catch
            {
                checkFailed = true;
                return false;
            }
        }
    }
}
