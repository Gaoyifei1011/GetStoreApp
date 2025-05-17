using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.Views.NotificationTips
{
    /// <summary>
    /// 复制剪贴应用内通知
    /// </summary>
    public sealed partial class CopyPasteMainNotificationTip : TeachingTip, INotifyPropertyChanged
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

        public CopyPasteMainNotificationTip(bool isSuccessfully)
        {
            InitializeComponent();
            IsSuccessfully = isSuccessfully;
        }
    }
}
