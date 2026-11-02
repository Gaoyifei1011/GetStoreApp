using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Text;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 许可证文字内容对话框
    /// </summary>
    internal sealed partial class LicenseDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：属性、集合与事件

        private string LicenseText
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(LicenseText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、集合与事件

        #region 第二部分：构造函数

        internal LicenseDialog()
        {
            InitializeComponent();
            LicenseText = Encoding.UTF8.GetString(ResourceService.GetEmbeddedData("Files/Assets/Embed/LICENSE"));
        }

        #endregion 第二部分：构造函数
    }
}
