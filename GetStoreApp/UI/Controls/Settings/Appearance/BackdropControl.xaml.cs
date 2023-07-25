using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Window;
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
    /// 设置页面：窗口背景材质设置控件
    /// </summary>
    public sealed partial class BackdropControl : Grid, INotifyPropertyChanged
    {
        public bool CanUseMicaBackdrop { get; set; }

        private BackdropModel _backdrop = BackdropService.AppBackdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                _backdrop = value;
                OnPropertyChanged();
            }
        }

        public List<BackdropModel> BackdropList { get; } = BackdropService.BackdropList;

        public event PropertyChangedEventHandler PropertyChanged;

        public BackdropControl()
        {
            InitializeComponent();
            int BuildNumber = InfoHelper.SystemVersion.Build;
            CanUseMicaBackdrop = BuildNumber >= 20000;
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        /// <summary>
        /// 背景色不可用时具体信息了解
        /// </summary>
        public void OnBackdropTipClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        public async void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender.As<ToggleMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(item.Tag)];
                await BackdropService.SetBackdropAsync(Backdrop);
                BackdropService.SetAppBackdrop();
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
