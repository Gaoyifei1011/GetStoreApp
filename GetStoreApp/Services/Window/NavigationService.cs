using GetStoreApp.Contracts.Services.Window;
using GetStoreApp.Models.Window;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Window
{
    public class NavigationService : INavigationService
    {
        public Frame NavigationFrame { get; set; }

        public List<NavigationModel> NavigationItemList { get; set; } = new List<NavigationModel>();

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType)
        {
            if (NavigationItemList.Exists(item => item.NavigationPage == navigationPageType))
            {
                NavigationFrame.Navigate(NavigationItemList.Find(item => item.NavigationPage == navigationPageType).NavigationPage, null, new DrillInNavigationTransitionInfo());
            }
        }

        /// <summary>
        /// 页面向后导航
        /// </summary>
        public void NavigationFrom()
        {
            if (NavigationFrame.CanGoBack)
            {
                NavigationFrame.GoBack();
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return NavigationFrame.CurrentSourcePageType;
        }

        public bool CanGoBack()
        {
            return NavigationFrame.CanGoBack;
        }
    }
}
