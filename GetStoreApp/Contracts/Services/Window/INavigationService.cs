using GetStoreApp.Models.Window;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Contracts.Services.Window
{
    public interface INavigationService
    {
        public Frame NavigationFrame { get; set; }

        List<NavigationModel> NavigationItemList { get; set; }

        void NavigateTo(Type navigationPageType);

        void NavigationFrom();

        Type GetCurrentPageType();

        bool CanGoBack();
    }
}
