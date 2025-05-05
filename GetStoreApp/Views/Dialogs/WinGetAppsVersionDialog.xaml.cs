using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation.Diagnostics;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// WinGet 应用版本信息对话框
    /// </summary>
    public sealed partial class WinGetAppsVersionDialog : ContentDialog, INotifyPropertyChanged
    {
        private SelectorBarItem _selectedItem;

        public SelectorBarItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private List<Type> PageList { get; } = [typeof(WinGetAppsCopyPage), typeof(WinGetAppsOperationPage), typeof(WinGetAppsCommandPage), typeof(WinGetAppsInfoPage)];

        private WinGetOperationKind WinGetOperationKind { get; }

        private WinGetPage WinGetPage { get; }

        private SearchAppsModel SearchApps { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetAppsVersionDialog(WinGetOperationKind winGetOptionKind, WinGetPage winGetPage, SearchAppsModel searchApps)
        {
            InitializeComponent();
            WinGetOperationKind = winGetOptionKind;
            WinGetPage = winGetPage;
            SearchApps = searchApps;
        }

        #region 第一部分：WinGet 应用版本信息对话框——挂载的事件

        /// <summary>
        /// 打开对话框时触发的事件
        /// </summary>
        private void OnOpened(object sender, ContentDialogOpenedEventArgs args)
        {
            WinGetAppsVersionFrame.Transitions = SuppressNavigationTransitionCollection;

            if (GetCurrentPageType() is null)
            {
                // 导航到复制页面
                if (WinGetOperationKind is WinGetOperationKind.SearchDownloadCopy || WinGetOperationKind is WinGetOperationKind.SearchInstallCopy || WinGetOperationKind is WinGetOperationKind.SearchRepairCopy)
                {
                    NavigateTo(PageList[0], SearchApps, null);
                }
                // 导航到操作页面
                else if (WinGetOperationKind is WinGetOperationKind.SearchDownload || WinGetOperationKind is WinGetOperationKind.SearchInstall || WinGetOperationKind is WinGetOperationKind.SearchRepair)
                {
                    NavigateTo(PageList[1], SearchApps, null);
                }
                // 导航到命令页面
                else if (WinGetOperationKind is WinGetOperationKind.SearchDownloadCommand || WinGetOperationKind is WinGetOperationKind.SearchInstallCommand || WinGetOperationKind is WinGetOperationKind.SearchRepairCommand)
                {
                    NavigateTo(PageList[2], SearchApps, null);
                }
                // 导航到版本信息页面
                else if (WinGetOperationKind is WinGetOperationKind.VersionInfo)
                {
                    NavigateTo(PageList[3], SearchApps, null);
                }
            }
        }

        /// <summary>
        /// 点击选择器栏发生的事件
        /// </summary>
        private void OnSelectorBarTapped(object sender, TappedRoutedEventArgs args)
        {
            if (sender is SelectorBarItem selectorBarItem && selectorBarItem.Tag is string tag)
            {
                int index = Convert.ToInt32(tag);
                int currentIndex = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

                if (index is 0 && !Equals(GetCurrentPageType(), PageList[0]))
                {
                    NavigateTo(PageList[0], SearchApps, index > currentIndex);
                }
                else if (index is 1 && !Equals(GetCurrentPageType(), PageList[1]))
                {
                    NavigateTo(PageList[1], SearchApps, index > currentIndex);
                }
                else if (index is 2 && !Equals(GetCurrentPageType(), PageList[2]))
                {
                    NavigateTo(PageList[2], SearchApps, index > currentIndex);
                }
                else if (index is 3 && !Equals(GetCurrentPageType(), PageList[3]))
                {
                    NavigateTo(PageList[3], SearchApps, index > currentIndex);
                }
            }
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Hide();
        }

        /// <summary>
        /// 导航完成后发生的事件
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < WinGetAppsSelectorBar.Items.Count)
            {
                SelectedItem = WinGetAppsSelectorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }
        }

        /// <summary>
        /// 导航失败时发生的事件
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < WinGetAppsSelectorBar.Items.Count)
            {
                SelectedItem = WinGetAppsSelectorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }

            LogService.WriteLog(LoggingLevel.Warning, string.Format(ResourceService.GetLocalized("WinGet/NavigationFailed"), args.SourcePageType.FullName), args.Exception);
        }

        #endregion 第一部分：WinGet 应用版本信息对话框——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    WinGetAppsVersionFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                WinGetAppsVersionFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, string.Format(ResourceService.GetLocalized("WinGet/NavigationFailed"), navigationPageType.FullName), e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return WinGetAppsVersionFrame.CurrentSourcePageType;
        }
    }
}
