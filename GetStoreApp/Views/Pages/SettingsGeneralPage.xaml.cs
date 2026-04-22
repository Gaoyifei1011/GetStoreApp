using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置通用选项页面
    /// </summary>
    public sealed partial class SettingsGeneralPage : Page, INotifyPropertyChanged
    {
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

        private ComboBoxItemModel _theme;

        public ComboBoxItemModel Theme
        {
            get { return _theme; }

            set
            {
                if (!string.Equals(_theme, value))
                {
                    _theme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme)));
                }
            }
        }

        private ComboBoxItemModel _backdrop;

        public ComboBoxItemModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                if (!Equals(_backdrop, value))
                {
                    _backdrop = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Backdrop)));
                }
            }
        }

        private bool _alwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set
            {
                if (!Equals(_alwaysShowBackdropValue, value))
                {
                    _alwaysShowBackdropValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlwaysShowBackdropValue)));
                }
            }
        }

        private bool _alwaysShowBackdropEnabled;

        public bool AlwaysShowBackdropEnabled
        {
            get { return _alwaysShowBackdropEnabled; }

            set
            {
                if (!Equals(_alwaysShowBackdropEnabled, value))
                {
                    _alwaysShowBackdropEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlwaysShowBackdropEnabled)));
                }
            }
        }

        private bool _advancedEffectsEnabled;

        public bool AdvancedEffectsEnabled
        {
            get { return _advancedEffectsEnabled; }

            set
            {
                if (!Equals(_advancedEffectsEnabled, value))
                {
                    _advancedEffectsEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdvancedEffectsEnabled)));
                }
            }
        }

        private ComboBoxItemModel _appLanguage;

        public ComboBoxItemModel AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                if (!Equals(_appLanguage, value))
                {
                    _appLanguage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppLanguage)));
                }
            }
        }

        private bool _topMostValue = TopMostService.TopMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set
            {
                if (!Equals(_topMostValue, value))
                {
                    _topMostValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TopMostValue)));
                }
            }
        }

        private List<ComboBoxItemModel> ThemeList { get; } = [];

        private List<ComboBoxItemModel> BackdropList { get; } = [];

        private ObservableCollection<ComboBoxItemModel> LanguageCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsGeneralPage()
        {
            InitializeComponent();

            AdvancedEffectsEnabled = uiSettings.AdvancedEffectsEnabled;
            ThemeList.Add(new ComboBoxItemModel() { SelectedValue = ThemeService.ThemeList[0], DisplayMember = ThemeDefaultString });
            ThemeList.Add(new ComboBoxItemModel() { SelectedValue = ThemeService.ThemeList[1], DisplayMember = ThemeLightAltString });
            ThemeList.Add(new ComboBoxItemModel() { SelectedValue = ThemeService.ThemeList[2], DisplayMember = ThemeDarkString });
            Theme = ThemeList.Find(item => Equals(Convert.ToString(item.SelectedValue), ThemeService.AppTheme));

            BackdropList.Add(new ComboBoxItemModel() { SelectedValue = BackdropService.BackdropList[0], DisplayMember = BackdropDefaultString });
            if (MicaController.IsSupported())
            {
                BackdropList.Add(new ComboBoxItemModel() { SelectedValue = BackdropService.BackdropList[1], DisplayMember = string.Format("{0} {1}", MicaString, BackdropMicaString) });
                BackdropList.Add(new ComboBoxItemModel() { SelectedValue = BackdropService.BackdropList[2], DisplayMember = string.Format("{0} {1}", MicaString, BackdropMicaAltString) });
            }
            if (DesktopAcrylicController.IsSupported())
            {
                BackdropList.Add(new ComboBoxItemModel() { SelectedValue = BackdropService.BackdropList[3], DisplayMember = string.Format("{0} {1}", DesktopAcrylicString, BackdropAcrylicString) });
                BackdropList.Add(new ComboBoxItemModel() { SelectedValue = BackdropService.BackdropList[4], DisplayMember = string.Format("{0} {1}", DesktopAcrylicString, BackdropAcrylicBaseString) });
                BackdropList.Add(new ComboBoxItemModel() { SelectedValue = BackdropService.BackdropList[5], DisplayMember = string.Format("{0} {1}", DesktopAcrylicString, BackdropAcrylicThinString) });
            }
            Backdrop = BackdropList.Find(item => Equals(Convert.ToString(item.SelectedValue), BackdropService.AppBackdrop));

            foreach (KeyValuePair<string, string> languageItem in LanguageService.LanguageList)
            {
                LanguageCollection.Add(new ComboBoxItemModel() { SelectedValue = languageItem.Key, DisplayMember = languageItem.Value });
            }

            foreach (ComboBoxItemModel languageItem in LanguageCollection)
            {
                if (string.Equals(Convert.ToString(languageItem.SelectedValue), LanguageService.AppLanguage.Key, StringComparison.OrdinalIgnoreCase))
                {
                    AppLanguage = languageItem;
                    break;
                }
            }

            AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !string.Equals(Convert.ToString(Backdrop.SelectedValue), Convert.ToString(BackdropList[0].SelectedValue));
            uiSettings.AdvancedEffectsEnabledChanged += OnAdvancedEffectsEnabledChanged;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
        }

        #region 第一部分：设置通用选项页面——挂载的事件

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        private void OnSystemThemeSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 主题选项修改后触发的事件
        /// </summary>
        private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel theme && !Equals(Theme, theme))
            {
                Theme = theme;
                ThemeService.SetTheme(Convert.ToString(Theme.SelectedValue));
            }
        }

        /// <summary>
        /// 背景色选项修改后触发的事件
        /// </summary>
        private void OnBackdropSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel backdrop && !Equals(Backdrop, backdrop))
            {
                Backdrop = backdrop;
                BackdropService.SetBackdrop(Convert.ToString(Backdrop.SelectedValue));
                AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !string.Equals(Convert.ToString(Backdrop.SelectedValue), Convert.ToString(BackdropList[0].SelectedValue));

                if (Equals(Backdrop, BackdropList[0]))
                {
                    AlwaysShowBackdropService.SetAlwaysShowBackdropValue(false);
                    AlwaysShowBackdropValue = false;
                }
            }
        }

        /// <summary>
        /// 打开系统背景色设置
        /// </summary>
        private void OnSystemBackdropSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:easeofaccess-visualeffects"));
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
        private void OnSystemLanguageSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:regionlanguage-languageoptions"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 是否开启始终显示背景色
        /// </summary>
        private void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            if (sender.As<ToggleSwitch>() is ToggleSwitch toggleSwitch)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdropValue(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 语言设置选项修改后触发的事件
        /// </summary>
        private async void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel language && !Equals(AppLanguage, language))
            {
                AppLanguage = language;

                LanguageService.SetLanguage(LanguageService.LanguageList.Find(item => string.Equals(Convert.ToString(AppLanguage.SelectedValue), item.Key)));
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.LanguageChange));
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            if (sender.As<ToggleSwitch>() is ToggleSwitch toggleSwitch)
            {
                TopMostService.SetTopMostValue(toggleSwitch.IsOn);
                TopMostValue = toggleSwitch.IsOn;
            }
        }

        #endregion 第一部分：设置通用选项页面——挂载的事件

        #region 第二部分：设置通用选项页面——自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                uiSettings.ColorValuesChanged -= OnAdvancedEffectsEnabledChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsGeneralPage), nameof(OnApplicationExit), 1, e);
            }
        }

        /// <summary>
        /// 在启用或禁用系统高级 UI 效果设置时发生的事件
        /// </summary>
        private void OnAdvancedEffectsEnabledChanged(UISettings sender, object args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                AdvancedEffectsEnabled = uiSettings.AdvancedEffectsEnabled;
                AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !string.Equals(Convert.ToString(Backdrop.SelectedValue), Convert.ToString(BackdropList[0].SelectedValue));
            });
        }

        #endregion 第二部分：设置通用选项页面——自定义事件
    }
}
