using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings.Advanced;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：应用安装方式设置控件
    /// </summary>
    public sealed partial class InstallModeControl : Grid, INotifyPropertyChanged
    {
        private GroupOptionsModel _installMode = InstallModeService.InstallMode;

        public GroupOptionsModel InstallMode
        {
            get { return _installMode; }

            set
            {
                _installMode = value;
                OnPropertyChanged();
            }
        }

        public List<GroupOptionsModel> InstallModeList => InstallModeService.InstallModeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public InstallModeControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(GroupOptionsModel selectedMember, GroupOptionsModel comparedMember)
        {
            return selectedMember.SelectedValue == comparedMember.SelectedValue;
        }

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        public void OnInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                InstallMode = InstallModeList[Convert.ToInt32(item.Tag)];
                InstallModeService.SetInstallMode(InstallMode);
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
