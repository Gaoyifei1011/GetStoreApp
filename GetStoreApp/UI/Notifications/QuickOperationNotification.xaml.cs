using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 快捷操作应用内通知
    /// </summary>
    public sealed partial class QuickOperationNotification : InAppNotification, INotifyPropertyChanged
    {
        private QuickOperationType _operationType;

        public QuickOperationType OperationType
        {
            get { return _operationType; }

            set
            {
                _operationType = value;
                OnPropertyChanged();
            }
        }

        private bool _isPinnedSuccessfully = false;

        public bool IsPinnedSuccessfully
        {
            get { return _isPinnedSuccessfully; }

            set
            {
                _isPinnedSuccessfully = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public QuickOperationNotification(FrameworkElement element, QuickOperationType operationType, bool isPinnedSuccessfully = false) : base(element)
        {
            InitializeComponent();
            OperationType = operationType;
            IsPinnedSuccessfully = isPinnedSuccessfully;
        }

        public bool ControlLoad(QuickOperationType operationType, bool isPinnedSuccessfully, string controlName)
        {
            if (controlName is "DesktopShortcutSuccess" && operationType is QuickOperationType.DesktopShortcut && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "DesktopShortcutFailed" && operationType is QuickOperationType.DesktopShortcut && !isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "StartScreenSuccess" && operationType is QuickOperationType.StartScreen && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "StartScreenFailed" && operationType is QuickOperationType.StartScreen && !isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "TaskbarSuccess" && operationType is QuickOperationType.Taskbar && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "TaskbarFailed" && operationType is QuickOperationType.Taskbar && !isPinnedSuccessfully)
            {
                return true;
            }
            else
            {
                return false;
            }
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
