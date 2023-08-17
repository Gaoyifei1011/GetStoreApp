using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings.Appearance;
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
    /// 设置页面：应用主题设置控件
    /// </summary>
    public sealed partial class ThemeControl : Grid, INotifyPropertyChanged
    {
        private GroupOptionsModel _theme = ThemeService.AppTheme;

        public GroupOptionsModel Theme
        {
            get { return _theme; }

            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        public List<GroupOptionsModel> ThemeList { get; } = ThemeService.ThemeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public ThemeControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(GroupOptionsModel selectedMember, GroupOptionsModel comparedMember)
        {
            return selectedMember.SelectedValue == comparedMember.SelectedValue;
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        public async void OnSystemThemeSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        public void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(item.Tag)];
                ThemeService.SetTheme(Theme);
                ThemeService.SetWindowTheme();
            }
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
