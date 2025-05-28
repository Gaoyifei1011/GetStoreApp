using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;
using Windows.UI.ViewManagement;

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

        private KeyValuePair<string, string> _theme;

        public KeyValuePair<string, string> Theme
        {
            get { return _theme; }

            set
            {
                if (!Equals(_theme, value))
                {
                    _theme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme)));
                }
            }
        }

        private KeyValuePair<string, string> _backdrop;

        public KeyValuePair<string, string> Backdrop
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

        private KeyValuePair<string, string> _appLanguage;

        public KeyValuePair<string, string> AppLanguage
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

        private List<KeyValuePair<string, string>> ThemeList { get; } = [];

        private List<KeyValuePair<string, string>> BackdropList { get; } = [];

        private ObservableCollection<LanguageModel> LanguageCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsGeneralPage()
        {
            InitializeComponent();

            AdvancedEffectsEnabled = uiSettings.AdvancedEffectsEnabled;
            ThemeList.Add(KeyValuePair.Create(ThemeService.ThemeList[0], ThemeDefaultString));
            ThemeList.Add(KeyValuePair.Create(ThemeService.ThemeList[1], ThemeLightAltString));
            ThemeList.Add(KeyValuePair.Create(ThemeService.ThemeList[2], ThemeDarkString));
            Theme = ThemeList.Find(item => string.Equals(item.Key, ThemeService.AppTheme, StringComparison.OrdinalIgnoreCase));

            BackdropList.Add(KeyValuePair.Create(BackdropService.BackdropList[0], BackdropDefaultString));
            BackdropList.Add(KeyValuePair.Create(BackdropService.BackdropList[1], BackdropMicaString));
            BackdropList.Add(KeyValuePair.Create(BackdropService.BackdropList[2], BackdropMicaAltString));
            BackdropList.Add(KeyValuePair.Create(BackdropService.BackdropList[3], BackdropAcrylicString));
            BackdropList.Add(KeyValuePair.Create(BackdropService.BackdropList[4], BackdropAcrylicBaseString));
            BackdropList.Add(KeyValuePair.Create(BackdropService.BackdropList[5], BackdropAcrylicThinString));
            Backdrop = BackdropList.Find(item => string.Equals(item.Key, BackdropService.AppBackdrop, StringComparison.OrdinalIgnoreCase));

            foreach (KeyValuePair<string, string> languageItem in LanguageService.LanguageList)
            {
                if (Equals(LanguageService.AppLanguage.Key, languageItem.Key))
                {
                    AppLanguage = languageItem;
                    LanguageCollection.Add(new LanguageModel()
                    {
                        LangaugeInfo = languageItem,
                        IsChecked = true
                    });
                }
                else
                {
                    LanguageCollection.Add(new LanguageModel()
                    {
                        LangaugeInfo = languageItem,
                        IsChecked = false
                    });
                }
            }

            AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !Equals(Backdrop.Key, BackdropList[0].Key);
            uiSettings.AdvancedEffectsEnabledChanged += OnAdvancedEffectsEnabledChanged;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 修改应用语言
        /// </summary>
        private async void OnLanguageExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (LanguageFlyout.IsOpen)
            {
                LanguageFlyout.Hide();
            }

            if (args.Parameter is LanguageModel language)
            {
                foreach (LanguageModel languageItem in LanguageCollection)
                {
                    languageItem.IsChecked = false;
                    if (Equals(language.LangaugeInfo.Key, languageItem.LangaugeInfo.Key))
                    {
                        AppLanguage = languageItem.LangaugeInfo;
                        languageItem.IsChecked = true;
                    }
                }

                LanguageService.SetLanguage(AppLanguage);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.LanguageChange));
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：设置通用选项页面——挂载的事件

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
        /// 主题修改设置
        /// </summary>
        private void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                Theme = ThemeList[Convert.ToInt32(tag)];
                ThemeService.SetTheme(Theme.Key);
            }
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        private void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                Backdrop = BackdropList[Convert.ToInt32(tag)];
                BackdropService.SetBackdrop(Backdrop.Key);
                AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !Equals(Backdrop.Key, BackdropList[0].Key);

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
        /// 开关按钮切换时修改相应设置
        /// </summary>
        private void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdropValue(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 语言设置菜单打开时自动定位到选中项
        /// </summary>
        private void OnLanguageFlyoutOpened(object sender, object args)
        {
            for (int index = 0; index < LanguageCollection.Count; index++)
            {
                if (LanguageCollection[index].IsChecked)
                {
                    LanguageListView.ScrollIntoView(LanguageCollection[index]);
                    break;
                }
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                TopMostService.SetTopMostValue(toggleSwitch.IsOn);
                TopMostValue = toggleSwitch.IsOn;
            }
        }

        #endregion 第二部分：设置通用选项页面——挂载的事件

        #region 第三部分：设置通用选项页面——自定义事件

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
                LogService.WriteLog(LoggingLevel.Error, "Unregister application exit event failed", e);
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
                AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !Equals(Backdrop.Key, BackdropList[0].Key);
            });
        }

        #endregion 第三部分：设置通用选项页面——自定义事件

        private string LocalizeDisplayNumber(KeyValuePair<string, string> selectedBackdrop)
        {
            int index = BackdropList.FindIndex(item => Equals(item.Key, selectedBackdrop.Key));

            if (index is 0)
            {
                return selectedBackdrop.Value;
            }
            else if (index is 1 or 2)
            {
                return string.Join(' ', MicaString, selectedBackdrop.Value);
            }
            else if (index is 3 or 4 or 5)
            {
                return string.Join(' ', DesktopAcrylicString, selectedBackdrop.Value);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
