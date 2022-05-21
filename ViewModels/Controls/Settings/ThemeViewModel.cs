using GetStoreApp.Services.Settings;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System.Windows.Input;

using Windows.UI.Xaml;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class ThemeViewModel : ObservableObject
    {
        // 主题设置
        private ElementTheme _elementTheme = ThemeSettings.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        // 主题修改Command
        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSettings.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        public ThemeViewModel()
        {
        }
    }
}
