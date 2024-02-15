using GetStoreAppWebView.Services.Root;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;
using System;
using System.Diagnostics;

namespace GetStoreAppWebView
{
    /// <summary>
    /// 表示 WinUI 3 Islands 当前应用程序及其可用服务
    /// </summary>
    public class WinUIApp : Application, IXamlMetadataProvider
    {
        private static XamlControlsXamlMetaDataProvider xamlMetaDataProvider = null;

        public WinUIApp()
        {
            DispatcherQueueController.CreateOnCurrentThread();
            WindowsXamlManager.InitializeForCurrentThread();

            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 根据指定完整类型名称，实现对基础类型映射的 XAML 架构上下文访问。
        /// </summary>
        public IXamlType GetXamlType(Type type)
        {
            return xamlMetaDataProvider.GetXamlType(type);
        }

        /// <summary>
        /// 根据指定完整类型名称，实现对基础类型映射的 XAML 架构上下文访问。
        /// </summary>
        public IXamlType GetXamlType(string fullName)
        {
            return xamlMetaDataProvider.GetXamlType(fullName);
        }

        /// <summary>
        /// 获取应用于上下文的 XMLNS (XAML 命名空间) 定义集。
        /// </summary>
        public XmlnsDefinition[] GetXmlnsDefinitions()
        {
            return xamlMetaDataProvider.GetXmlnsDefinitions();
        }

        /// <summary>
        /// 应用程序启动时的资源
        /// </summary>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            XamlControlsXamlMetaDataProvider.Initialize();
            xamlMetaDataProvider = new XamlControlsXamlMetaDataProvider();

            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Microsoft.UI.Xaml/Themes/themeresources.xaml") });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/AppBarButton.xaml") });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/MenuFlyout.xaml") });
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(EventLogEntryType.Error, "Unknown unhandled exception.", args.Exception);
            (System.Windows.Application.Current as WPFApp).Dispose();
        }
    }
}
