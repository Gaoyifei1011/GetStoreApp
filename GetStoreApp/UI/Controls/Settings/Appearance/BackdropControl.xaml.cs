using GetStoreApp.Helpers.Root;
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
    /// 设置页面：窗口背景材质设置控件
    /// </summary>
    public sealed partial class BackdropControl : Grid, INotifyPropertyChanged
    {
        public bool CanUseMicaBackdrop { get; set; }

        private GroupOptionsModel _backdrop = BackdropService.AppBackdrop;

        public GroupOptionsModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                _backdrop = value;
                OnPropertyChanged();
            }
        }

        public List<GroupOptionsModel> BackdropList { get; } = BackdropService.BackdropList;

        public event PropertyChangedEventHandler PropertyChanged;

        public BackdropControl()
        {
            InitializeComponent();
            int BuildNumber = InfoHelper.SystemVersion.Build;
            CanUseMicaBackdrop = BuildNumber >= 20000;
        }

        public bool IsItemChecked(GroupOptionsModel selectedMember, GroupOptionsModel comparedMember)
        {
            return selectedMember.SelectedValue == comparedMember.SelectedValue;
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        public void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(item.Tag)];
                BackdropService.SetBackdrop(Backdrop);
                BackdropService.SetAppBackdrop();
            }
        }

        /// <summary>
        /// 打开系统背景色设置
        /// </summary>
        public async void OnSystemBackdropSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:easeofaccess-visualeffects"));
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
