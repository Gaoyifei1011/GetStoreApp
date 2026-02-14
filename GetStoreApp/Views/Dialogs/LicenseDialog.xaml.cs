using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Text;
using Windows.Security.Cryptography;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 许可证文字内容对话框
    /// </summary>
    public sealed partial class LicenseDialog : ContentDialog, INotifyPropertyChanged
    {
        private string _licenseText = Encoding.UTF8.GetString(ResourceService.GetEmbeddedData("Files/Assets/Embed/LICENSE"));

        public string LicenseText
        {
            get { return _licenseText; }

            set
            {
                if (!string.Equals(_licenseText, value))
                {
                    _licenseText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LicenseText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LicenseDialog()
        {
            InitializeComponent();
        }
    }
}
