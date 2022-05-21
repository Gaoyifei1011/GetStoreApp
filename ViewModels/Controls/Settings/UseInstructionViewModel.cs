using GalaSoft.MvvmLight.Messaging;
using GetStoreApp.Services.Settings;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class UseInstructionViewModel : ObservableObject
    {
        // 主页“使用说明”按钮显示状态
        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get
            {
                _useInsVisValue = UseInstructionSettings.UseInsVisValue;
                return _useInsVisValue;
            }
            set
            {
                UseInstructionSettings.SetUseInsVisValue(value);
                Messenger.Default.Send(value, "UseInsVisValue");
                SetProperty(ref _useInsVisValue, value);
            }
        }

        public UseInstructionViewModel()
        {
        }
    }
}
