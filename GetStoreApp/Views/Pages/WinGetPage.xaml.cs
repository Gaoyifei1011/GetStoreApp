using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 程序包页面
    /// </summary>
    public sealed partial class WinGetPage : Page
    {
        public WinGetPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化 WinGet 程序包视图模型
        /// </summary>
        private void OnInitializeSuccessLoaded()
        {
            SearchApps.ViewModel.WinGetVMInstance = ViewModel;
            UpgradableApps.ViewModel.WinGetVMInstance = ViewModel;
        }

        /// <summary>
        /// 判断 WinGet 程序包是否存在
        /// </summary>
        public bool IsWinGetExisted(bool isOfficialVersionExisted, bool isDevVersionExisted, bool needReverseValue)
        {
            bool result = isOfficialVersionExisted || isDevVersionExisted;
            if (needReverseValue)
            {
                return !result;
            }
            else
            {
                return result;
            }
        }
    }
}
