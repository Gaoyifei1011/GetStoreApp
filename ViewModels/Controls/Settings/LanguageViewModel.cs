using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LanguageViewModel : ObservableRecipient
    {
        // 语言设置
        private string _selectedLanguage = LanguageService.PriLangCodeName;

        public string SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }
            set
            {
                SetProperty(ref _selectedLanguage, value);
                LanguageService.SetLanguage(value);
            }
        }

        private ICommand _launchSettingsInstalledAppsCommand;

        public ICommand LaunchSettingsInstalledAppsCommand
        {
            get { return _launchSettingsInstalledAppsCommand; }
            set { SetProperty(ref _launchSettingsInstalledAppsCommand, value); }
        }

        // 语言列表
        public List<LanguageData> LanguageList = SettingsViewModel.LanguageList;

        public LanguageViewModel()
        {
            LaunchSettingsInstalledAppsCommand = new RelayCommand(async () => { await LaunchSettingsInstalledAppsClickedAsync(); });
        }

        private static async Task LaunchSettingsInstalledAppsClickedAsync()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        }
    }
}