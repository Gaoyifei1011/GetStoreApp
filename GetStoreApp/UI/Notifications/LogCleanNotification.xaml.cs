using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 日志记录清除通知
    /// </summary>
    public sealed partial class LogCleanNotification : InAppNotification, INotifyPropertyChanged
    {
        private bool _setResult = false;

        public bool SetResult
        {
            get { return _setResult; }

            set
            {
                _setResult = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LogCleanNotification(FrameworkElement element, bool setResult = false) : base(element)
        {
            InitializeComponent();
            SetResult = setResult;
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
