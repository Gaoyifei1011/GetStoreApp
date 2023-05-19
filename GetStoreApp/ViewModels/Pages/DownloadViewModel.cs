using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;

namespace GetStoreApp.ViewModels.Pages
{
    /// <summary>
    /// 下载页面视图模型
    /// </summary>
    public sealed class DownloadViewModel : ViewModelBase
    {
        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set
            {
                _useInsVisValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        public void LearnMore()
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        public void OpenDownloadSettings()
        {
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }
    }
}
