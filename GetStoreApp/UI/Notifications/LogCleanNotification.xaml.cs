using GetStoreApp.Views.CustomControls.Notifications;
using System.ComponentModel;

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SetResult)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LogCleanNotification(bool setResult = false)
        {
            InitializeComponent();
            SetResult = setResult;
        }
    }
}
