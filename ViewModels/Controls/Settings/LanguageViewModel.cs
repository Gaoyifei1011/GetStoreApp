using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LanguageViewModel : ObservableRecipient
    {
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

        public IAsyncRelayCommand LaunchSettingsInstalledAppsCommand { get; set; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        });

        public IReadOnlyList<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;
    }
}
