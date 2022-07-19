using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LanguageViewModel : ObservableRecipient
    {
        private readonly ILanguageService LanguageService;
        private readonly INavigationService NavigationService;

        private string _language;

        public string Language
        {
            get { return _language; }

            set { SetProperty(ref _language, value); }
        }

        public List<LanguageModel> LanguageList { get; }

        public IAsyncRelayCommand LanguageTipCommand { get; set; }

        public IAsyncRelayCommand LanguageSelectCommand { get; set; }

        public LanguageViewModel(ILanguageService languageService,INavigationService navigationService)
        {
            LanguageService = languageService;
            NavigationService = navigationService;

            Language = LanguageService.AppLanguage;
            LanguageList = LanguageService.LanguageList;

            LanguageTipCommand = new AsyncRelayCommand(async () =>
            {
                NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                await Task.CompletedTask;
            });

            LanguageSelectCommand = new AsyncRelayCommand(async () =>
            {
                await LanguageService.SetLanguageAsync(Language);
            });
        }
    }
}
