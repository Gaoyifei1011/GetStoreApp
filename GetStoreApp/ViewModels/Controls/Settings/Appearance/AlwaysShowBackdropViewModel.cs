using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public sealed class AlwaysShowBackdropViewModel : ViewModelBase
    {
        private bool _alwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set
            {
                _alwaysShowBackdropValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 开关按钮切换时修改相应设置
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await AlwaysShowBackdropService.SetAlwaysShowBackdropAsync(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }
    }
}
