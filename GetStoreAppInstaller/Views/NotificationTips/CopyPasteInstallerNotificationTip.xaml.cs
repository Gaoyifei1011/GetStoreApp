using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreAppInstaller.Views.NotificationTips
{
    /// <summary>
    /// 复制剪贴应用内通知
    /// </summary>
    public sealed partial class CopyPasteInstallerNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        private bool _isSuccessfully;

        public bool IsSuccessfully
        {
            get { return _isSuccessfully; }

            set
            {
                if (!Equals(_isSuccessfully, value))
                {
                    _isSuccessfully = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessfully)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CopyPasteInstallerNotificationTip(bool isSuccessfully = false)
        {
            InitializeComponent();
            IsSuccessfully = isSuccessfully;
        }
    }
}
