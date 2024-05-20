using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Dialogs.Settings;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        private bool _isCleaning = false;

        public bool IsCleaning
        {
            get { return _isCleaning; }

            set
            {
                if (!Equals(_isCleaning, value))
                {
                    _isCleaning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCleaning)));
                }
            }
        }

        private List<TraceCleanupModel> TraceCleanupList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public TraceCleanupPromptDialog()
        {
            InitializeComponent();

            foreach (TraceCleanupModel traceCleanupItem in ResourceService.TraceCleanupList)
            {
                traceCleanupItem.IsSelected = false;
                traceCleanupItem.IsCleanFailed = false;
                traceCleanupItem.PropertyChanged += (sender, args) =>
                {
                    IsSelected = TraceCleanupList.Exists(item => item.IsSelected);
                };

                TraceCleanupList.Add(traceCleanupItem);
            }
        }

        /// <summary>
        /// 痕迹清理
        /// </summary>
        private void OnCleanupNowClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
            {
                traceCleanupItem.IsCleanFailed = false;
            }

            IsCleaning = true;
            TraceCleanup();
        }

        private bool IsButtonEnabled(bool isSelected, bool isCleaning)
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
        private void TraceCleanup()
        {
            Task.Run(async () =>
            {
                List<CleanKind> selectedCleanList = [];

                foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
                {
                    if (traceCleanupItem.IsSelected is true)
                    {
                        selectedCleanList.Add(traceCleanupItem.InternalName);
                    }
                }

                List<Tuple<CleanKind, bool>> cleanSuccessfullyDict = [];
                foreach (CleanKind cleanArgs in selectedCleanList)
                {
                    // 清理并反馈回结果，修改相应的状态信息
                    bool cleanReusult = TraceCleanupService.CleanAppTraceAsync(cleanArgs);
                    cleanSuccessfullyDict.Add(new Tuple<CleanKind, bool>(cleanArgs, cleanReusult));
                }

                await Task.Delay(1000);

                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (Tuple<CleanKind, bool> cleanArgsTuple in cleanSuccessfullyDict)
                    {
                        foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
                        {
                            if (traceCleanupItem.InternalName.Equals(cleanArgsTuple.Item1))
                            {
                                traceCleanupItem.IsCleanFailed = !cleanArgsTuple.Item2;
                                break;
                            }
                        }
                    }

                    IsCleaning = false;
                });
            });
        }
    }
}
