using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Notifications;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinRT;

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
                OnPropertyChanged();
            }
        }

        public List<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

        public event PropertyChangedEventHandler PropertyChanged;

        public LauguageControl()
        {
            InitializeComponent();

            for (int index = 0; index < LanguageList.Count; index++)
            {
                LanguageModel languageItem = LanguageList[index];
                ToggleMenuFlyoutItem toggleMenuFlyoutItem = new ToggleMenuFlyoutItem()
                {
                    Text = languageItem.DisplayName,
                    Style = ResourceDictionaryHelper.MenuFlyoutResourceDict["ToggleMenuFlyoutItemStyle"] as Style,
                    Tag = index
                };
                if (AppLanguage.InternalName == LanguageList[index].InternalName)
                {
                    toggleMenuFlyoutItem.IsChecked = true;
                }

                toggleMenuFlyoutItem.Click += async (sender, args) =>
                {
                    foreach (MenuFlyoutItemBase menuFlyoutItemBase in LanguageFlyout.Items)
                    {
                        ToggleMenuFlyoutItem toggleMenuFlyoutItem = menuFlyoutItemBase.As<ToggleMenuFlyoutItem>();
                        if (toggleMenuFlyoutItem is not null && toggleMenuFlyoutItem.IsChecked)
                        {
                            toggleMenuFlyoutItem.IsChecked = false;
                        }
                    }

                    int selectedIndex = Convert.ToInt32(sender.As<ToggleMenuFlyoutItem>().Tag);
                    LanguageFlyout.Items[selectedIndex].As<ToggleMenuFlyoutItem>().IsChecked = true;

                    if (AppLanguage.InternalName != LanguageList[selectedIndex].InternalName)
                    {
                        AppLanguage = LanguageList[selectedIndex];
                        await LanguageService.SetLanguageAsync(AppLanguage);
                        new LanguageChangeNotification(this).Show();
                    }
                };
                LanguageFlyout.Items.Add(toggleMenuFlyoutItem);
            }
        }

        /// <summary>
        /// 语言设置说明
        /// </summary>
        public void OnLanguageTipClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
