using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 微软商店页面
    /// </summary>
    internal sealed partial class StorePage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private bool isInitialized;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private StoreControl _storeControl;

        private StoreControl StoreControl
        {
            get { return _storeControl; }

            set
            {
                if (!Equals(_storeControl, value))
                {
                    _storeControl = value;
                    PropertyChanged?.Invoke(this, new(nameof(StoreControl)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal StorePage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await StoreSelector.InitializeStoreSelectorAsync(this);
                QueryLinksResult.InitializeQueryLinksResult(this);
                SearchAppsResult.InitializeSearchAppsResult(this);
            }

            if (StoreSelector is not null && args.Parameter is List<string> dataList)
            {
                StoreControl = StoreControl.StoreSelector;
                StoreSelector.UpdateData(dataList);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 关闭使用说明浮出栏
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (StoreSplitView.IsPaneOpen)
            {
                StoreSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 桌面程序启动参数说明
        /// </summary>
        private async void OnDesktopLaunchClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            await MainWindow.Current.ShowDialogAsync(new DesktopStartupArgsDialog());
        }

        /// <summary>
        /// 检查网络
        /// </summary>
        private void OnCheckNetworkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            CheckNetwork();
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private void OnTroubleShootClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            OpenTroubleShoot();
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        private async void OnDownloadSettingsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            await Task.Delay(300);
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.Download);
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 显示使用说明
        /// </summary>
        internal void ShowUseInstruction()
        {
            if (!StoreSplitView.IsPaneOpen)
            {
                StoreSplitView.IsPaneOpen = true;
            }
        }

        /// <summary>
        /// 检查网络
        /// </summary>
        private void CheckNetwork()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:network"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开疑难解答
        /// </summary>
        private void OpenTroubleShoot()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:troubleshoot"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 获取当前选择的商店控件
        /// </summary>
        private Visibility GetStoreControlVisibility(StoreControl selectedStoreControl, StoreControl comparedStoreControl)
        {
            return Equals(selectedStoreControl, comparedStoreControl) ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
