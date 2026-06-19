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
    public sealed partial class StorePage : Page, INotifyPropertyChanged
    {
        private bool isInitialized;

        private StoreControl _storeControl;

        public StoreControl StoreControl
        {
            get { return _storeControl; }

            set
            {
                if (!Equals(_storeControl, value))
                {
                    _storeControl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StoreControl)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public StorePage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

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
            }

            if (StoreSelector is not null && args.Parameter is List<string> dataList)
            {
                StoreControl = StoreControl.StoreSelector;
                StoreSelector.UpdateData(dataList);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：应用商店页面——挂载的事件

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
        private void OnCheckNetWorkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private void OnTroubleShootClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
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

        #endregion 第二部分：应用商店页面——挂载的事件

        /// <summary>
        /// 显示使用说明
        /// </summary>
        public void ShowUseInstruction()
        {
            if (!StoreSplitView.IsPaneOpen)
            {
                StoreSplitView.IsPaneOpen = true;
            }
        }

        /// <summary>
        /// 获取当前选择的商店控件
        /// </summary>
        private Visibility GetSelectedStoreControl(StoreControl selectedStoreControl, StoreControl comparedStoreControl)
        {
            return Equals(selectedStoreControl, comparedStoreControl) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
