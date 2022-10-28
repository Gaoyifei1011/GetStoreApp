using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Models.Notifications;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public class LanguageViewModel : ObservableRecipient
    {
        private ILanguageService LanguageService { get; } = IOCHelper.GetService<ILanguageService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

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
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
        });

        // 应用默认语言修改
        public IRelayCommand LanguageSelectCommand => new RelayCommand(async () =>
        {
            await LanguageService.SetLanguageAsync(Language);
            WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
            {
                NotificationArgs = InAppNotificationArgs.LanguageSettings,
                NotificationValue = new object[] { true }
            }));
        });

        public LanguageViewModel()
        {
            Language = LanguageService.AppLanguage;
        }
    }
}
