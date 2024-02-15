using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 许可证文字内容对话框
    /// </summary>
    public sealed partial class LicenseDialog : ContentDialog, INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public LicenseDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化许可证对话框时加载的内容
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            LicenseText = Encoding.UTF8.GetString(await ResourceService.GetEmbeddedDataAsync("Files/EmbedAssets/LICENSE"));
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
