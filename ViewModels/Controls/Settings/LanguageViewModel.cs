using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LanguageViewModel : ObservableRecipient
    {
        private readonly ILanguageService LanguageService;

        private string _language;

        public string Language
        {
            get { return _language; }

            set { SetProperty(ref _language, value); }
        }

        public List<LanguageModel> LanguageList { get; }

        public IAsyncRelayCommand LaunchSettingsInstalledAppsCommand { get; set; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        });

        public IAsyncRelayCommand LanguageSelectCommand { get; set; }

        public LanguageViewModel(ILanguageService languageService)
        {
            LanguageService = languageService;

            Language = LanguageService.AppLanguage;
            LanguageList = LanguageService.LanguageList;

            LanguageSelectCommand = new AsyncRelayCommand(async () =>
            {
                await LanguageService.SetLanguageAsync(Language);
            });
        }
    }
}
