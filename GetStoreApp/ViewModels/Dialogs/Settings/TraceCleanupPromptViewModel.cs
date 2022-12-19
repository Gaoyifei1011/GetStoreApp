using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Controls.Web;
using GetStoreApp.Models.Dialogs.CommonDialogs.Settings;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs.Settings
{
    public sealed class TraceCleanupPromptViewModel : ViewModelBase
    {
        public List<TraceCleanupModel> TraceCleanupList { get; set; } = new List<TraceCleanupModel>();

        private bool _isFirstInitialize = true;

        public bool IsFirstInitialize
        {
            get { return _isFirstInitialize; }

            set
            {
                _isFirstInitialize = value;
                OnPropertyChanged();
            }
        }

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

        // 痕迹清理
        public IRelayCommand CleanupNowCommand => new RelayCommand(async () =>
        {
            IsFirstInitialize = false;
            foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
            {
                traceCleanupItem.IsCleanFailed = false;
            }
            IsCleaning = true;
            await TraceCleanupAsync();
            await Task.Delay(1000);
            IsCleaning = false;
        });

        // 关闭窗口
        public IRelayCommand CloseWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            foreach (TraceCleanupModel traceCleanupItem in TraceCleanupList)
            {
                traceCleanupItem.PropertyChanged -= OnPropertyChanged;
            }
            dialog.Hide();
        });

        /// <summary>
        /// 对话框加载完成后让内容对话框的烟雾层背景（SmokeLayerBackground）覆盖到标题栏中
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            ContentDialog dialog = sender as ContentDialog;

            if (dialog is not null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(dialog);

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject current = VisualTreeHelper.GetChild(parent, i);
                    if (current is Rectangle { Name: "SmokeLayerBackground" } background)
                    {
                        background.Margin = new Thickness(0);
                        background.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty, OnMarginChanged);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 重置内容对话框烟雾背景距离顶栏的间隔
        /// </summary>
        private void OnMarginChanged(DependencyObject sender, DependencyProperty property)
        {
            if (property == FrameworkElement.MarginProperty)
            {
                sender.ClearValue(property);
            }
        }

        /// <summary>
        /// 初始化清理列表信息
        /// </summary>
        public void InitializeTraceCleanupList()
        {
            foreach (TraceCleanupModel traceCleanupItem in ResourceService.TraceCleanupList)
            {
                if (traceCleanupItem.InternalName == CleanArgs.WebCache)
                {
                    if (!WebView2Helper.IsInstalled())
                    {
                        continue;
                    }
                }

                traceCleanupItem.IsSelected = false;
                traceCleanupItem.IsCleanFailed = false;
                traceCleanupItem.PropertyChanged += OnPropertyChanged;

                TraceCleanupList.Add(traceCleanupItem);
            }
        }

        /// <summary>
        /// 痕迹清理
        /// </summary>
        private async Task TraceCleanupAsync()
        {
            List<CleanArgs> SelectedCleanList = new List<CleanArgs>(TraceCleanupList.Where(item => item.IsSelected == true).Select(item => item.InternalName));

            foreach (CleanArgs cleanupArgs in SelectedCleanList)
            {
                // 清理并反馈回结果，修改相应的状态信息
                bool CleanReusult = await TraceCleanupService.CleanAppTraceAsync(cleanupArgs);

                if (cleanupArgs == CleanArgs.History)
                {
                    Messenger.Default.Send(true, MessageToken.History);
                }

                TraceCleanupList[TraceCleanupList.IndexOf(TraceCleanupList.First(item => item.InternalName == cleanupArgs))].IsCleanFailed = !CleanReusult;
            }
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            IsSelected = TraceCleanupList.Exists(item => item.IsSelected);
        }
    }
}
