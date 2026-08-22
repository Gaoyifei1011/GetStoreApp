using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.System;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置应用安装页面
    /// </summary>
    internal sealed partial class SettingsAppInstallPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：属性、集合与事件

        private bool _allowUnsignedPackage;

        private bool AllowUnsignedPackage
        {
            get { return _allowUnsignedPackage; }

            set
            {
                if (!Equals(_allowUnsignedPackage, value))
                {
                    _allowUnsignedPackage = value;
                    PropertyChanged?.Invoke(this, new(nameof(AllowUnsignedPackage)));
                }
            }
        }

        private bool _forceAppShutdown;

        private bool ForceAppShutdown
        {
            get { return _forceAppShutdown; }

            set
            {
                if (!Equals(_forceAppShutdown, value))
                {
                    _forceAppShutdown = value;
                    PropertyChanged?.Invoke(this, new(nameof(ForceAppShutdown)));
                }
            }
        }

        private bool _forceTargetAppShutdown;

        private bool ForceTargetAppShutdown
        {
            get { return _forceTargetAppShutdown; }

            set
            {
                if (!Equals(_forceTargetAppShutdown, value))
                {
                    _forceTargetAppShutdown = value;
                    PropertyChanged?.Invoke(this, new(nameof(ForceTargetAppShutdown)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、集合与事件

        #region 第二部分：构造函数

        internal SettingsAppInstallPage()
        {
            InitializeComponent();
        }

        #endregion 第二部分：构造函数

        #region 第三部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面后触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            AllowUnsignedPackage = AppInstallService.AllowUnsignedPackage;
            ForceAppShutdown = AppInstallService.ForceAppShutdown;
            ForceTargetAppShutdown = AppInstallService.ForceTargetAppShutdown;
        }

        #endregion 第三部分：父类虚方法重写

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 打开开发者选项
        /// </summary>
        private void OnOpenDevelopersClicked(object sender, RoutedEventArgs args)
        {
            OpenDevelpers();
        }

        /// <summary>
        /// 是否允许安装未签名的安装包
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnAllowUnsignedPackageToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(AllowUnsignedPackage, toggleSwitch.IsOn))
            {
                AllowUnsignedPackage = toggleSwitch.IsOn;
                AppInstallService.SetAllowUnsignedPackage(toggleSwitch.IsOn);
                AllowUnsignedPackage = AppInstallService.AllowUnsignedPackage;
            }
        }

        /// <summary>
        /// 是否在安装应用时强制关闭与包关联的进程
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnForceAppShutdownToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(ForceAppShutdown, toggleSwitch.IsOn))
            {
                ForceAppShutdown = toggleSwitch.IsOn;
                AppInstallService.SetForceAppShutdown(toggleSwitch.IsOn);
                ForceAppShutdown = AppInstallService.ForceAppShutdown;
            }
        }

        /// <summary>
        /// 是否在安装应用时强制关闭与包关联的进程
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnForceTargetAppShutdownToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(ForceTargetAppShutdown, toggleSwitch.IsOn))
            {
                ForceTargetAppShutdown = toggleSwitch.IsOn;
                AppInstallService.SetForceTargetAppShutdown(toggleSwitch.IsOn);
                ForceTargetAppShutdown = AppInstallService.ForceTargetAppShutdown;
            }
        }

        /// <summary>
        /// 配置应用包存储卷
        /// </summary>
        private void OnConfigurationClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                // 导航到应用包存储卷配置页面
                settingsPage.NavigateTo(settingsPage.PageList[2], null, true);
            }
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 打开开发者选项
        /// </summary>
        private void OpenDevelpers()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:developers"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
