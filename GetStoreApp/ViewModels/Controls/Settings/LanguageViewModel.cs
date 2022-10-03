using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models.Notification;
using GetStoreApp.Models.Settings;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
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
        public IAsyncRelayCommand LanguageTipCommand => new AsyncRelayCommand(async () =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
            await Task.CompletedTask;
        });

        // 应用默认语言修改
        public IAsyncRelayCommand LanguageSelectCommand => new AsyncRelayCommand(async () =>
        {
            await LanguageService.SetLanguageAsync(Language);
            WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
            {
                NotificationContent = InAppNotificationContent.LanguageSettings,
                NotificationValue = new object[] { true }
            }));
        });

        public LanguageViewModel()
        {
            Language = LanguageService.AppLanguage;
        }
    }
}
