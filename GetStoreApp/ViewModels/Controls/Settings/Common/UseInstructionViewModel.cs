using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public sealed class UseInstructionViewModel : ViewModelBase
    {
        private bool _useInsVisValue = UseInstructionService.UseInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set
            {
                _useInsVisValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// “使用说明”按钮显示设置
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await UseInstructionService.SetUseInsVisValueAsync(toggleSwitch.IsOn);
                UseInsVisValue = toggleSwitch.IsOn;
            }
        }
    }
}
