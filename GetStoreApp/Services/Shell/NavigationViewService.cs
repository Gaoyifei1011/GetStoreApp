using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 导航视图服务
    /// </summary>
    public class NavigationViewService : INavigationViewService
    {
        private NavigationView _navigationView;

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();
        private IPageService PageService { get; } = IOCHelper.GetService<IPageService>();

        public IList<object> MenuItems => _navigationView.MenuItems;

        public IList<object> FooterMenuItems => _navigationView.FooterMenuItems;

        public NavigationViewItem SettingsItem => (NavigationViewItem)_navigationView.SettingsItem;

        public NavigationViewItem GetSelectedItem(Type pageType) => GetSelectedItem(MenuItems.Concat(FooterMenuItems), pageType);

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => NavigationService.GoBack();

        public void Initialize(NavigationView navigationView)
        {
            _navigationView = navigationView;
            _navigationView.BackRequested += OnBackRequested;
            _navigationView.ItemInvoked += OnItemInvoked;
        }

        public void UnregisterEvents()
        {
            _navigationView.BackRequested -= OnBackRequested;
            _navigationView.ItemInvoked -= OnItemInvoked;
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.NavigateTo(typeof(SettingsViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
            }
            else
            {
                NavigationViewItem selectedItem = args.InvokedItemContainer as NavigationViewItem;

                if (selectedItem.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
                {
                    NavigationService.NavigateTo(pageKey, null, new DrillInNavigationTransitionInfo(), false);
                }
            }
        }

        private NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (NavigationViewItem navigationViewItem in menuItems.OfType<NavigationViewItem>())
            {
                if (IsMenuItemForPageType(navigationViewItem, pageType))
                {
                    return navigationViewItem;
                }

                NavigationViewItem selectedChild = GetSelectedItem(navigationViewItem.MenuItems, pageType);
                if (selectedChild is not null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            if (menuItem.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
            {
                return PageService.GetPageType(pageKey) == sourcePageType;
            }

            return false;
        }
    }
}
