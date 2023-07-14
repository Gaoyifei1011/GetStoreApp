using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Advanced;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using WinRT;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：应用关闭方式设置控件
    /// </summary>
    public sealed partial class AppExitControl : Grid, INotifyPropertyChanged
    {
        private AppExitModel _appExit = AppExitService.AppExit;

        public AppExitModel AppExit
        {
            get { return _appExit; }

            set
            {
                _appExit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppExit)));
            }
        }

        public List<AppExitModel> AppExitList { get; } = AppExitService.AppExitList;

        public event PropertyChangedEventHandler PropertyChanged;

        public AppExitControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        /// <summary>
        /// 应用退出方式设置
        /// </summary>
        public async void OnAppExitSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender.As<RadioMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                AppExit = AppExitList[Convert.ToInt32(item.Tag)];
                await AppExitService.SetAppExitAsync(AppExit);
            }
        }
    }
}
