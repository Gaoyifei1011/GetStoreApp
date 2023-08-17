using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：界面语言设置控件
    /// </summary>
    public sealed partial class LauguageControl : Grid, INotifyPropertyChanged
    {
        private GroupOptionsModel _appLanguage = LanguageService.AppLanguage;

        public GroupOptionsModel AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                _appLanguage = value;
                OnPropertyChanged();
            }
        }

        public List<GroupOptionsModel> LanguageList { get; } = LanguageService.LanguageList;

        public event PropertyChangedEventHandler PropertyChanged;

        public LauguageControl()
        {
            InitializeComponent();

            for (int index = 0; index < LanguageList.Count; index++)
            {
                GroupOptionsModel languageItem = LanguageList[index];
                ToggleMenuFlyoutItem toggleMenuFlyoutItem = new ToggleMenuFlyoutItem()
                {
                    Text = languageItem.DisplayMember,
                    Style = ResourceDictionaryHelper.MenuFlyoutResourceDict["ToggleMenuFlyoutItemStyle"] as Style,
                    Tag = index
                };

                if (AppLanguage.SelectedValue == LanguageList[index].SelectedValue)
                {
                    toggleMenuFlyoutItem.IsChecked = true;
                }

                toggleMenuFlyoutItem.Click += (sender, args) =>
                {
                    foreach (MenuFlyoutItemBase menuFlyoutItemBase in LanguageFlyout.Items)
                    {
                        ToggleMenuFlyoutItem toggleMenuFlyoutItem = menuFlyoutItemBase as ToggleMenuFlyoutItem;
                        if (toggleMenuFlyoutItem is not null && toggleMenuFlyoutItem.IsChecked)
                        {
                            toggleMenuFlyoutItem.IsChecked = false;
                        }
                    }

                    int selectedIndex = Convert.ToInt32((sender as ToggleMenuFlyoutItem).Tag);
                    (LanguageFlyout.Items[selectedIndex] as ToggleMenuFlyoutItem).IsChecked = true;

                    if (AppLanguage.SelectedValue != LanguageList[selectedIndex].SelectedValue)
                    {
                        AppLanguage = LanguageList[selectedIndex];
                        LanguageService.SetLanguage(AppLanguage);
                        new LanguageChangeNotification(this).Show();
                    }
                };
                LanguageFlyout.Items.Add(toggleMenuFlyoutItem);
            }
        }

        public async void OnSystemLanguageSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:regionlanguage-languageoptions"));
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
