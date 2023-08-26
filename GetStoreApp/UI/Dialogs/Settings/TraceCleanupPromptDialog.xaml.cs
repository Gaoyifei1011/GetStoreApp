﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Dialogs.Settings;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// 痕迹清理对话框
    /// </summary>
    public sealed partial class TraceCleanupPromptDialog : ContentDialog, INotifyPropertyChanged
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isCleaning = false;

        public bool IsCleaning
        {
            get { return _isCleaning; }

            set
            {
                _isCleaning = value;
                OnPropertyChanged();
            }
        }

        public List<TraceCleanupModel> TraceCleanupList { get; set; } = new List<TraceCleanupModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public TraceCleanupPromptDialog()
        {
            InitializeComponent();

            foreach (TraceCleanupModel traceCleanupItem in ResourceService.TraceCleanupList)
            {
                traceCleanupItem.IsSelected = false;
                traceCleanupItem.IsCleanFailed = false;
                traceCleanupItem.PropertyChanged += OnTraceCleanupPropertyChanged;

                TraceCleanupList.Add(traceCleanupItem);
            }
        }

        public bool IsButtonEnabled(bool isSelected, bool isCleaning)
        {
            if (isCleaning)
            {
                return false;
            }
            else
            {
                if (isSelected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 痕迹清理
        /// </summary>
        public void OnCleanupNowClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
            {
                traceCleanupItem.IsCleanFailed = false;
            }

            IsCleaning = true;
            TraceCleanup();
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnTraceCleanupPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            IsSelected = TraceCleanupList.Exists(item => item.IsSelected);
        }

        /// <summary>
        /// 痕迹清理
        /// </summary>
        private void TraceCleanup()
        {
            Task.Run(async () =>
            {
                List<CleaKind> SelectedCleanList = new List<CleaKind>(TraceCleanupList.Where(item => item.IsSelected is true).Select(item => item.InternalName));

                List<Tuple<CleaKind, bool>> cleanSuccessfullyDict = new List<Tuple<CleaKind, bool>>();
                foreach (CleaKind cleanArgs in SelectedCleanList)
                {
                    // 清理并反馈回结果，修改相应的状态信息
                    bool cleanReusult = await TraceCleanupService.CleanAppTraceAsync(cleanArgs);
                    cleanSuccessfullyDict.Add(new Tuple<CleaKind, bool>(cleanArgs, cleanReusult));
                }

                await Task.Delay(1000);

                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (Tuple<CleaKind, bool> cleanArgsTuple in cleanSuccessfullyDict)
                    {
                        TraceCleanupList[TraceCleanupList.IndexOf(TraceCleanupList.First(item => item.InternalName == cleanArgsTuple.Item1))].IsCleanFailed = !cleanArgsTuple.Item2;
                    }

                    IsCleaning = false;
                });
            });
        }
    }
}
