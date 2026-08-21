using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreAppInstaller.Views.NotificationTips
{
    /// <summary>
    /// 复制剪贴应用内通知
    /// </summary>
    internal sealed partial class CopyPasteInstallerNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        #region 第一部分：属性、集合与事件

        private bool _isSuccessfully;

        private bool IsSuccessfully
        {
            get { return _isSuccessfully; }

            set
            {
                if (!Equals(_isSuccessfully, value))
                {
                    _isSuccessfully = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSuccessfully)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、集合与事件

        #region 第二部分：构造函数

        internal CopyPasteInstallerNotificationTip(bool isSuccessfully = false)
        {
            InitializeComponent();
            IsSuccessfully = isSuccessfully;
        }

        #endregion 第二部分：构造函数
    }
}
