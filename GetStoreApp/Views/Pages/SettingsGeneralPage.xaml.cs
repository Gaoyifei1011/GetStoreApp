using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;
using Windows.UI.ViewManagement;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置通用选项页面
    /// </summary>
    internal sealed partial class SettingsGeneralPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string BackdropAcrylicString = ResourceService.GetLocalized("SettingsGeneral/BackdropAcrylic");
        private readonly string BackdropAcrylicBaseString = ResourceService.GetLocalized("SettingsGeneral/BackdropAcrylicBase");
        private readonly string BackdropAcrylicThinString = ResourceService.GetLocalized("SettingsGeneral/BackdropAcrylicThin");
        private readonly string BackdropDefaultString = ResourceService.GetLocalized("SettingsGeneral/BackdropDefault");
        private readonly string BackdropMicaString = ResourceService.GetLocalized("SettingsGeneral/BackdropMica");
        private readonly string BackdropMicaAltString = ResourceService.GetLocalized("SettingsGeneral/BackdropMicaAlt");
        private readonly string DesktopAcrylicString = ResourceService.GetLocalized("SettingsGeneral/DesktopAcrylic");
        private readonly string MicaString = ResourceService.GetLocalized("SettingsGeneral/Mica");
        private readonly string ThemeDarkString = ResourceService.GetLocalized("SettingsGeneral/ThemeDark");
        private readonly string ThemeDefaultString = ResourceService.GetLocalized("SettingsGeneral/ThemeDefault");
        private readonly string ThemeLightAltString = ResourceService.GetLocalized("SettingsGeneral/ThemeLight");
        private readonly UISettings uiSettings = new();
        private bool isInitialized;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private ComboBoxItemModel _theme;

        private ComboBoxItemModel Theme
        {
            get { return _theme; }

            set
            {
                if (!string.Equals(_theme, value))
                {
                    _theme = value;
                    PropertyChanged?.Invoke(this, new(nameof(Theme)));
                }
            }
        }

        private ComboBoxItemModel _backdrop;

        private ComboBoxItemModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                if (!Equals(_backdrop, value))
                {
                    _backdrop = value;
                    PropertyChanged?.Invoke(this, new(nameof(Backdrop)));
                }
            }
        }

        private bool _alwaysShowBackdrop;

        private bool AlwaysShowBackdrop
        {
            get { return _alwaysShowBackdrop; }

            set
            {
                if (!Equals(_alwaysShowBackdrop, value))
                {
                    _alwaysShowBackdrop = value;
                    PropertyChanged?.Invoke(this, new(nameof(AlwaysShowBackdrop)));
                }
            }
        }

        private bool _alwaysShowBackdropEnabled;

        private bool AlwaysShowBackdropEnabled
        {
            get { return _alwaysShowBackdropEnabled; }

            set
            {
                if (!Equals(_alwaysShowBackdropEnabled, value))
                {
                    _alwaysShowBackdropEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(AlwaysShowBackdropEnabled)));
                }
            }
        }

        private bool _advancedEffectsEnabled;

        private bool AdvancedEffectsEnabled
        {
            get { return _advancedEffectsEnabled; }

            set
            {
                if (!Equals(_advancedEffectsEnabled, value))
                {
                    _advancedEffectsEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(AdvancedEffectsEnabled)));
                }
            }
        }

        private ComboBoxItemModel _appLanguage;

        private ComboBoxItemModel AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                if (!Equals(_appLanguage, value))
                {
                    _appLanguage = value;
                    PropertyChanged?.Invoke(this, new(nameof(AppLanguage)));
                }
            }
        }

        private bool _topMost;

        private bool TopMost
        {
            get { return _topMost; }

            set
            {
                if (!Equals(_topMost, value))
                {
                    _topMost = value;
                    PropertyChanged?.Invoke(this, new(nameof(TopMost)));
                }
            }
        }

        private List<ComboBoxItemModel> ThemeList { get; } = [];

        private ObservableCollection<ComboBoxItemModel> BackdropCollection { get; } = [];

        private ObservableCollection<ComboBoxItemModel> LanguageCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal SettingsGeneralPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面后触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (!isInitialized)
            {
                isInitialized = true;
                MountSettingsEvent();
            }

            AdvancedEffectsEnabled = uiSettings.AdvancedEffectsEnabled;
            AlwaysShowBackdrop = AlwaysShowBackdropService.AlwaysShowBackdrop;
            TopMost = TopMostService.TopMost;
            Theme = ThemeList.Find(item => Equals(Convert.ToString(item.SelectedValue), ThemeService.AppTheme));
            foreach (ComboBoxItemModel backdropItem in BackdropCollection)
            {
                if (string.Equals(Convert.ToString(backdropItem.SelectedValue), BackdropService.AppBackdrop, StringComparison.OrdinalIgnoreCase))
                {
                    Backdrop = backdropItem;
                    break;
                }
            }
            foreach (ComboBoxItemModel languageItem in LanguageCollection)
            {
                if (string.Equals(Convert.ToString(languageItem.SelectedValue), LanguageService.AppLanguage.Key, StringComparison.OrdinalIgnoreCase))
                {
                    AppLanguage = languageItem;
                    break;
                }
            }
            AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !string.Equals(Convert.ToString(Backdrop.SelectedValue), Convert.ToString(BackdropCollection[0].SelectedValue));
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        private void OnSystemThemeSettingsClicked(object sender, RoutedEventArgs args)
        {
            OpenSystemThemeSettings();
        }

        /// <summary>
        /// 主题选项修改后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(Theme, comboBox.SelectedItem))
            {
                Theme = comboBox.SelectedItem is ComboBoxItemModel theme ? theme : null;

                if (Theme is not null)
                {
                    ThemeService.SetTheme(Convert.ToString(Theme.SelectedValue));
                }

                Theme = ThemeList.Find(item => Equals(Convert.ToString(item.SelectedValue), ThemeService.AppTheme));
            }
        }

        /// <summary>
        /// 背景色选项修改后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnBackdropSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(Backdrop, comboBox.SelectedItem))
            {
                Backdrop = comboBox.SelectedItem is ComboBoxItemModel backdrop ? backdrop : null;

                if (Backdrop is not null)
                {
                    BackdropService.SetBackdrop(Convert.ToString(Backdrop.SelectedValue));
                }

                foreach (ComboBoxItemModel backdropItem in BackdropCollection)
                {
                    if (string.Equals(Convert.ToString(backdropItem.SelectedValue), BackdropService.AppBackdrop, StringComparison.OrdinalIgnoreCase))
                    {
                        Backdrop = backdropItem;
                        break;
                    }
                }
                AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !string.Equals(Convert.ToString(Backdrop.SelectedValue), Convert.ToString(BackdropCollection[0].SelectedValue));

                if (Equals(Backdrop, BackdropCollection[0]))
                {
                    AlwaysShowBackdropService.SetAlwaysShowBackdrop(false);
                    AlwaysShowBackdrop = false;
                }
            }
        }

        /// <summary>
        /// 打开系统背景色设置
        /// </summary>
        private void OnSystemBackdropSettingsClicked(object sender, RoutedEventArgs args)
        {
            OpenSystemBackdropSettings();
        }

        /// <summary>
        /// 打开系统语言设置
        /// </summary>
        private void OnSystemLanguageSettingsClicked(object sender, RoutedEventArgs args)
        {
            OpenSystemLanguageSettings();
        }

        /// <summary>
        /// 是否开启始终显示背景色
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(AlwaysShowBackdrop, toggleSwitch.IsOn))
            {
                AlwaysShowBackdrop = toggleSwitch.IsOn;
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(toggleSwitch.IsOn);
                AlwaysShowBackdrop = AlwaysShowBackdropService.AlwaysShowBackdrop;
            }
        }

        /// <summary>
        /// 语言设置选项修改后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private async void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(AppLanguage, comboBox.SelectedItem))
            {
                AppLanguage = comboBox.SelectedItem is ComboBoxItemModel language ? language : null;

                if (AppLanguage is not null)
                {
                    LanguageService.SetLanguage(LanguageService.LanguageList.Find(item => string.Equals(Convert.ToString(AppLanguage.SelectedValue), item.Key)));
                }

                foreach (ComboBoxItemModel languageItem in LanguageCollection)
                {
                    if (string.Equals(Convert.ToString(languageItem.SelectedValue), LanguageService.AppLanguage.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        AppLanguage = languageItem;
                        break;
                    }
                }
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.LanguageChange));
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(TopMost, toggleSwitch.IsOn))
            {
                TopMost = toggleSwitch.IsOn;
                TopMostService.SetTopMost(toggleSwitch.IsOn);
                TopMost = TopMostService.TopMost;
            }
        }

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            DismountSettingsEvent();
        }

        /// <summary>
        /// 在启用或禁用系统高级 UI 效果设置时发生的事件
        /// </summary>
        private void OnAdvancedEffectsEnabledChanged(UISettings sender, object args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                AdvancedEffectsEnabled = uiSettings.AdvancedEffectsEnabled;
                AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !string.Equals(Convert.ToString(Backdrop.SelectedValue), Convert.ToString(BackdropCollection[0].SelectedValue));
            });
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            ThemeList.Add(new() { SelectedValue = ThemeService.ThemeList[0], DisplayMember = ThemeDefaultString });
            ThemeList.Add(new() { SelectedValue = ThemeService.ThemeList[1], DisplayMember = ThemeLightAltString });
            ThemeList.Add(new() { SelectedValue = ThemeService.ThemeList[2], DisplayMember = ThemeDarkString });

            BackdropCollection.Add(new() { SelectedValue = BackdropService.BackdropList[0], DisplayMember = BackdropDefaultString });
            if (MicaController.IsSupported())
            {
                BackdropCollection.Add(new() { SelectedValue = BackdropService.BackdropList[1], DisplayMember = string.Format("{0} {1}", MicaString, BackdropMicaString) });
                BackdropCollection.Add(new() { SelectedValue = BackdropService.BackdropList[2], DisplayMember = string.Format("{0} {1}", MicaString, BackdropMicaAltString) });
            }
            if (DesktopAcrylicController.IsSupported())
            {
                BackdropCollection.Add(new() { SelectedValue = BackdropService.BackdropList[3], DisplayMember = string.Format("{0} {1}", DesktopAcrylicString, BackdropAcrylicString) });
                BackdropCollection.Add(new() { SelectedValue = BackdropService.BackdropList[4], DisplayMember = string.Format("{0} {1}", DesktopAcrylicString, BackdropAcrylicBaseString) });
                BackdropCollection.Add(new() { SelectedValue = BackdropService.BackdropList[5], DisplayMember = string.Format("{0} {1}", DesktopAcrylicString, BackdropAcrylicThinString) });
            }

            foreach (KeyValuePair<string, string> languageItem in LanguageService.LanguageList)
            {
                LanguageCollection.Add(new() { SelectedValue = languageItem.Key, DisplayMember = languageItem.Value });
            }
        }

        /// <summary>
        /// 挂载设置事件
        /// </summary>
        private void MountSettingsEvent()
        {
            uiSettings.AdvancedEffectsEnabledChanged += OnAdvancedEffectsEnabledChanged;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
        }

        /// <summary>
        /// 卸载设置事件
        /// </summary>
        private void DismountSettingsEvent()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                uiSettings.ColorValuesChanged -= OnAdvancedEffectsEnabledChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsGeneralPage), nameof(DismountSettingsEvent), 1, e);
            }
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        private void OpenSystemThemeSettings()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:colors"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开系统背景色设置
        /// </summary>
        private void OpenSystemBackdropSettings()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:easeofaccess-visualeffects"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开系统语言设置
        /// </summary>
        private void OpenSystemLanguageSettings()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:regionlanguage-languageoptions"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
