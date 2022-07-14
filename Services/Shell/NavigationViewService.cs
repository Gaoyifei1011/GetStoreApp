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
        private readonly INavigationService NavigationService;
        private readonly IPageService PageService;
        private NavigationView _navigationView;

        public IList<object> MenuItems
            => _navigationView.MenuItems;

        public IList<object> FooterMenuItems
            => _navigationView.FooterMenuItems;

        public object SettingsItem
            => _navigationView.SettingsItem;

        public NavigationViewService(INavigationService navigationService, IPageService pageService)
        {
            NavigationService = navigationService;
            PageService = pageService;
        }

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

        public NavigationViewItem GetSelectedItem(Type pageType)
            => GetSelectedItem(MenuItems.Concat(FooterMenuItems), pageType);

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
            => NavigationService.GoBack();

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.NavigateTo(typeof(SettingsViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            }
            else
            {
                NavigationViewItem selectedItem = args.InvokedItemContainer as NavigationViewItem;

                if (selectedItem.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
                {
                    NavigationService.NavigateTo(pageKey, null, new DrillInNavigationTransitionInfo());
                }
            }
        }

        private NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (NavigationViewItem item in menuItems.OfType<NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                NavigationViewItem selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
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
