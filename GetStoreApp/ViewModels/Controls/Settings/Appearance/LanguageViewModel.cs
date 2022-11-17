using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.UI.Notifications;
using GetStoreApp.Views.Pages;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public class LanguageViewModel : ObservableRecipient
    {
        private ILanguageService LanguageService { get; } = ContainerHelper.GetInstance<ILanguageService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public List<LanguageModel> LanguageList => LanguageService.LanguageList;

        private LanguageModel _language;

        public LanguageModel Language
        {
            get { return _language; }

            set { SetProperty(ref _language, value); }
        }

        // 语言设置说明
        public IRelayCommand LanguageTipCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutPage));
        });

        // 应用默认语言修改
        public IRelayCommand LanguageSelectCommand => new RelayCommand(async () =>
        {
            await LanguageService.SetLanguageAsync(Language);
            new LanguageChangeNotification(true).Show();
        });

        public LanguageViewModel()
        {
            Language = LanguageService.AppLanguage;
        }
    }
}
