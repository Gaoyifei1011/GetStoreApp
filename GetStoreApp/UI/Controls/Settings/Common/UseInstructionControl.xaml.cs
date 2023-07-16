using GetStoreApp.Services.Controls.Settings.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using WinRT;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：使用说明按钮显示设置控件
    /// </summary>
    public sealed partial class UseInstructionControl : Grid, INotifyPropertyChanged
    {
        private bool _useInsVisValue = UseInstructionService.UseInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set
            {
                _useInsVisValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseInsVisValue)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public UseInstructionControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// “使用说明”按钮显示设置
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender.As<ToggleSwitch>();
            if (toggleSwitch is not null)
            {
                await UseInstructionService.SetUseInsVisValueAsync(toggleSwitch.IsOn);
                UseInsVisValue = toggleSwitch.IsOn;
            }
        }
    }
}
