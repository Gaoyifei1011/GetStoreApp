using GetStoreApp.Contracts.Services;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetStoreApp.Services.Shell
{
    public class NavigationViewService : INavigationViewService
    {
        private readonly INavigationService _navigationService;
        private readonly IPageService _pageService;
        private NavigationView _navigationView;

        public IList<object> MenuItems
            => _navigationView.MenuItems;

        public IList<object> FooterMenuItems
            => _navigationView.FooterMenuItems;

        public object SettingsItem
            => _navigationView.SettingsItem;

        public NavigationViewService(INavigationService navigationService, IPageService pageService)
        {
            _navigationService = navigationService;
            _pageService = pageService;
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
            => _navigationService.GoBack();

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _navigationService.NavigateTo(typeof(SettingsViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            }
            else
            {
                var selectedItem = args.InvokedItemContainer as NavigationViewItem;
                var pageKey = selectedItem.GetValue(NavigationHelper.NavigateToProperty) as string;

                if (pageKey != null)
                {
                    _navigationService.NavigateTo(pageKey, null, new DrillInNavigationTransitionInfo());
                }
            }
        }

        private NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageKey = menuItem.GetValue(NavigationHelper.NavigateToProperty) as string;
            if (pageKey != null)
            {
                return _pageService.GetPageType(pageKey) == sourcePageType;
            }

            return false;
        }
    }
}