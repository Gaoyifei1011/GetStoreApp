using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public sealed class LanguageViewModel : ViewModelBase
    {
        public List<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

        private LanguageModel _language = LanguageService.AppLanguage;

        public LanguageModel Language
        {
            get { return _language; }

            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        // 语言设置说明
        public IRelayCommand LanguageTipCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutPage));
        });

        /// <summary>
        /// 应用默认语言修改
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                await LanguageService.SetLanguageAsync(Language);
                new LanguageChangeNotification(true).Show();
            }
        }
    }
}
