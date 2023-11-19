using GetStoreAppWebView.Services.Root;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Diagnostics;

namespace GetStoreAppWebView
{
    /// <summary>
    /// 网页浏览器程序
    /// </summary>
    public class App : Application, IXamlMetadataProvider, IDisposable
    {
        private bool isDisposed;

        private List<IXamlMetadataProvider> providers;

        public App()
        {
            providers = new List<IXamlMetadataProvider>
            {
                new XamlControlsXamlMetaDataProvider(),
            };

            DispatcherQueueController.CreateOnCurrentThread();
            WindowsXamlManager.InitializeForCurrentThread();

            UnhandledException += OnUnhandledException;
        }

        public IXamlType GetXamlType(Type type)
        {
            foreach (var provider in providers)
            {
                IXamlType result = provider.GetXamlType(type);
                if (result is not null)
                {
                    return result;
                }
            }
            return null;
        }

        public IXamlType GetXamlType(string fullName)
        {
            foreach (var provider in providers)
            {
                IXamlType result = provider.GetXamlType(fullName);
                if (result is not null)
                {
                    return result;
                }
            }
            return null;
        }

        public XmlnsDefinition[] GetXmlnsDefinitions()
        {
            return providers.SelectMany(p => p.GetXmlnsDefinitions()).ToArray();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 应用程序启动时的资源
        /// </summary>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Microsoft.UI.Xaml/Themes/themeresources.xaml") });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/MenuFlyout.xaml") });
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Error, "Unknown unhandled exception.", args.Exception);
            Dispose();
        }

        ~App()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    isDisposed = true;
                    Program.MainWindow.Close();
                    Environment.Exit(0);
                }
            }
        }
    }
}
