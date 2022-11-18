using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public sealed class TopMostViewModel : ViewModelBase
    {
        private bool _topMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set
            {
                _topMostValue = value;
                OnPropertyChanged();
            }
        }

        public TopMostViewModel()
        {
            TopMostValue = TopMostService.TopMostValue;
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await TopMostService.SetTopMostValueAsync(toggleSwitch.IsOn);
                await TopMostService.SetAppTopMostAsync();
                TopMostValue = toggleSwitch.IsOn;
            }
        }
    }
}
