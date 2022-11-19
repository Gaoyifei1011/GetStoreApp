﻿using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public sealed class BackdropViewModel : ViewModelBase
    {
        public List<BackdropModel> BackdropList { get; } = new List<BackdropModel>();

        private BackdropModel _backdrop = BackdropService.AppBackdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                _backdrop = value;
                OnPropertyChanged();
            }
        }

        public bool BackdropHelp { get; }

        // 背景色不可用时具体信息了解
        public IRelayCommand BackdropTipCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(SettingsPage));
        });

        public BackdropViewModel()
        {
            ulong BuildNumber = InfoHelper.GetSystemVersion()["BuildNumber"];

            if (BuildNumber < 22000)
            {
                BackdropHelp = true;
                BackdropList.Add(BackdropService.BackdropList[0]);
                BackdropList.Add(BackdropService.BackdropList[3]);
            }
            else
            {
                BackdropHelp = false;
                BackdropList = BackdropService.BackdropList;
            }
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                await BackdropService.SetBackdropAsync(Backdrop);
                await BackdropService.SetAppBackdropAsync();
                WeakReferenceMessenger.Default.Send(new BackdropChangedMessage(Backdrop));
            }
        }
    }
}