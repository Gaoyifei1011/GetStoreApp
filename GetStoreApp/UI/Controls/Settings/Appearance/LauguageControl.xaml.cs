using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Notifications;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：界面语言设置控件
    /// </summary>
    public sealed partial class LauguageControl : Grid, INotifyPropertyChanged
    {
        private LanguageModel _appLanguage = LanguageService.AppLanguage;

        public LanguageModel AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                _appLanguage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppLanguage)));
            }
        }

        public List<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

        public event PropertyChangedEventHandler PropertyChanged;

        public LauguageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 语言设置说明
        /// </summary>
        public void OnLanguageTipClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 应用默认语言修改
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                AppLanguage = args.AddedItems[0] as LanguageModel;
                await LanguageService.SetLanguageAsync(AppLanguage);
                new LanguageChangeNotification(this).Show();
            }
        }
    }
}
