using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Models;
using GetStoreApp.Services.App;
using GetStoreApp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LanguageViewModel : ObservableRecipient
    {
        private string _selectedLanguage = LanguageService.PriLangCodeName;

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }

            set { SetProperty(ref _selectedLanguage, value); }
        }

        public IAsyncRelayCommand LaunchSettingsInstalledAppsCommand { get; set; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        });

        public IAsyncRelayCommand LanguageSelectCommand { get; set; }

        public IReadOnlyList<LanguageModel> LanguageList { get; }

        public LanguageViewModel()
        {
            LanguageSelectCommand = new AsyncRelayCommand(LanguageSelectAsync);
        }

        /// <summary>
        /// 设置界面语言
        /// </summary>
        private async Task LanguageSelectAsync()
        {
            LanguageService.SetLanguage(SelectedLanguage);
            await Task.CompletedTask;
        }
    }
}
