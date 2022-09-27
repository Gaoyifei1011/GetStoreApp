using CommunityToolkit.Mvvm.ComponentModel;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 页面服务
    /// </summary>
    public class PageService : IPageService
    {
        private Dictionary<string, Type> Pages { get; } = new Dictionary<string, Type>();

        public PageService()
        {
            Configure<HomeViewModel, HomePage>();
            Configure<DownloadViewModel, DownloadPage>();
            Configure<HistoryViewModel, HistoryPage>();
            Configure<WebViewModel, WebPage>();
            Configure<AboutViewModel, AboutPage>();
            Configure<SettingsViewModel, SettingsPage>();
        }

        public Type GetPageType(string key)
        {
            Type pageType;
            lock (Pages)
            {
                if (!Pages.TryGetValue(key, out pageType))
                {
                    //throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
                }
            }

            return pageType;
        }

        private void Configure<VM, V>()
            where VM : ObservableObject
            where V : Page
        {
            lock (Pages)
            {
                string key = typeof(VM).FullName;
                if (Pages.ContainsKey(key))
                {
                    throw new ArgumentException($"The key {key} is already configured in PageService");
                }

                Type type = typeof(V);
                if (Pages.Any(p => p.Value == type))
                {
                    throw new ArgumentException($"This type is already configured with key {Pages.First(p => p.Value == type).Key}");
                }

                Pages.Add(key, type);
            }
        }
    }
}
