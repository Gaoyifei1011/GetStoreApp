using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Helpers;
using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class ThemeViewModel : ObservableRecipient
    {
        // 主题设置
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

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

                            BackdropHelper.CurrentTheme = ElementTheme;

                            BackdropHelper.SetBackdrop();

                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }
    }
}
