using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置应用安装器页面
    /// </summary>
    public sealed partial class SettingsAppInstallerPage : Page, INotifyPropertyChanged
    {
        private bool _allowUnsignedPackageValue = AppInstallService.AllowUnsignedPackageValue;

        public bool AllowUnsignedPackageValue
        {
            get { return _allowUnsignedPackageValue; }

            set
            {
                if (!Equals(_allowUnsignedPackageValue, value))
                {
                    _allowUnsignedPackageValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllowUnsignedPackageValue)));
                }
            }
        }

        private bool _forceAppShutdownValue = AppInstallService.ForceAppShutdownValue;

        public bool ForceAppShutdownValue
        {
            get { return _forceAppShutdownValue; }

            set
            {
                if (!Equals(_forceAppShutdownValue, value))
                {
                    _forceAppShutdownValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceAppShutdownValue)));
                }
            }
        }

        private bool _forceTargetAppShutdownValue = AppInstallService.ForceTargetAppShutdownValue;

        public bool ForceTargetAppShutdownValue
        {
            get { return _forceTargetAppShutdownValue; }

            set
            {
                if (!Equals(_forceTargetAppShutdownValue, value))
                {
                    _forceTargetAppShutdownValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceTargetAppShutdownValue)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsAppInstallerPage()
        {
            InitializeComponent();
        }

        #region 第一部分：设置应用安装器页面——挂载的事件

        /// <summary>
        /// 打开开发者选项
        /// </summary>
        private void OnOpenDevelopersClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:developers"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 是否允许安装未签名的安装包
        /// </summary>
        private void OnAllowUnsignedPackageToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AppInstallService.SetAllowUnsignedPackageValue(toggleSwitch.IsOn);
                AllowUnsignedPackageValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否在安装应用时强制关闭与包关联的进程
        /// </summary>
        private void OnForceAppShutdownToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AppInstallService.SetForceAppShutdownValue(toggleSwitch.IsOn);
                ForceAppShutdownValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否在安装应用时强制关闭与包关联的进程
        /// </summary>
        private void OnForceTargetAppShutdownToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AppInstallService.SetForceTargetAppShutdownValue(toggleSwitch.IsOn);
                ForceTargetAppShutdownValue = toggleSwitch.IsOn;
            }
        }

        #endregion 第一部分：设置应用安装器页面——挂载的事件
    }
}
