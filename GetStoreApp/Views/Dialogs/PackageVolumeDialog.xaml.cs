using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.UI.Text;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 应用包存储卷对话框
    /// </summary>
    public sealed partial class PackageVolumeDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string PackageVolumeInfoString = ResourceService.GetLocalized("Dialog/PackageVolumeInfo");
        private readonly string PackageVolumeAddString = ResourceService.GetLocalized("Dialog/PackageVolumeAdd");
        private readonly PackageManager PackageManager;
        private readonly PackageModel Package;

        private bool _isPrimaryEnabled;

        public bool IsPrimaryEnabled
        {
            get { return _isPrimaryEnabled; }

            set
            {
                if (!Equals(_isPrimaryEnabled, value))
                {
                    _isPrimaryEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPrimaryButtonEnabled)));
                }
            }
        }

        public List<Type> PageList { get; } = [typeof(PackageVolumeInfoPage), typeof(PackageVolumeAddPage)];

        public ObservableCollection<ContentLinkInfo> BreadCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public PackageVolumeDialog(PackageManager packageManager, PackageModel package)
        {
            InitializeComponent();
            PackageManager = packageManager;
            Package = package;
        }

        #region 第一部分：应用包存储卷对话框——挂载的事件

        /// <summary>
        /// 打开对话框时触发的事件
        /// </summary>
        private void OnOpened(object sender, ContentDialogOpenedEventArgs args)
        {
            PackageVolumeFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], new List<object>() { this, PackageManager, Package }, null);
            }
        }

        /// <summary>
        /// 点击返回到应用包存储卷信息页面
        /// </summary>
        private void OnBackClicked(object sender, RoutedEventArgs args)
        {
            if (BreadCollection.Count is 2 && Equals(GetCurrentPageType(), PageList[1]))
            {
                NavigateTo(PageList[0], null, false);
            }
        }

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(object sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is ContentLinkInfo contentLinkInfo && BreadCollection.Count is 2 && Equals(contentLinkInfo.SecondaryText, BreadCollection[0].SecondaryText))
            {
                NavigateTo(PageList[0], null, false);
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            if (BreadCollection.Count is 0 && Equals(GetCurrentPageType(), PageList[0]))
            {
                BreadCollection.Add(new ContentLinkInfo()
                {
                    DisplayText = PackageVolumeInfoString,
                    SecondaryText = "PackageVolumeInfo"
                });
            }
            else if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[1]))
            {
                BreadCollection.Add(new ContentLinkInfo()
                {
                    DisplayText = PackageVolumeAddString,
                    SecondaryText = "PackageVolumeAdd"
                });
            }
            else if (BreadCollection.Count is 2 && Equals(GetCurrentPageType(), PageList[0]))
            {
                BreadCollection.RemoveAt(1);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeDialog), nameof(OnNavigationFailed), 1, args.Exception);
        }

        #endregion 第一部分：应用包存储卷对话框——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    PackageVolumeFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                PackageVolumeFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeDialog), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取能否返回到应用包存储卷信息页面
        /// </summary>
        private bool GetCanBack(int count)
        {
            return count > 1;
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        private Type GetCurrentPageType()
        {
            return PackageVolumeFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        public object GetFrameContent()
        {
            return PackageVolumeFrame.Content;
        }
    }
}
