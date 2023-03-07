using GetStoreApp.Properties;
using GetStoreApp.ViewModels.Base;
using System.Text;

namespace GetStoreApp.ViewModels.Dialogs.About
{
    /// <summary>
    /// 应用许可证对话框视图模型
    /// </summary>
    public sealed class LicenseViewModel : ViewModelBase
    {
        private string _licenseText;

        public string LicenseText
        {
            get { return _licenseText; }

            set
            {
                _licenseText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 对话框加载完成后让内容对话框的烟雾层背景（SmokeLayerBackground）覆盖到标题栏中，并初始化许可证信息
        /// </summary>
        public void OnLoading(object sender, object args)
        {
            LicenseText = Encoding.UTF8.GetString(Resources.LICENSE);
        }
    }
}
