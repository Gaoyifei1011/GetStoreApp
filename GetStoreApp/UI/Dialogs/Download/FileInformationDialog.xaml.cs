using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.UI.Dialogs.Download
{
    /// <summary>
    /// 文件信息对话框
    /// </summary>
    public sealed partial class FileInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private bool isInitialized;

        private string FileName { get; set; }

        private string FilePath { get; set; }

        private string FileSize { get; set; }

        private bool _isLoadCompleted = false;

        public bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadCompleted)));
                }
            }
        }

        private string _fileSHA256 = string.Empty;

        public string FileSHA256
        {
            get { return _fileSHA256; }

            set
            {
                if (!Equals(_fileSHA256, value))
                {
                    _fileSHA256 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileSHA256)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FileInformationDialog(CompletedModel completedItem)
        {
            InitializeComponent();
            FileName = completedItem.FileName;
            FilePath = completedItem.FilePath;
            FileSize = FileSizeHelper.ConvertFileSizeToString(completedItem.TotalSize);
        }

        /// <summary>
        /// 文件信息对话框初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                string fileShA256 = await Task.Run(async () =>
                {
                    return await IOHelper.GetFileSHA256Async(FilePath);
                });

                FileSHA256 = fileShA256;
                IsLoadCompleted = true;
            }
        }

        /// <summary>
        /// 加载完成前禁用关闭对话框
        /// </summary>
        private void OnClosing(object sender, ContentDialogClosingEventArgs args)
        {
            if (!IsLoadCompleted)
            {
                args.Cancel = true;
            }
        }

        /// <summary>
        /// 复制文件信息
        /// </summary>
        private async void OnCopyFileInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            List<string> copyFileInformationCopyStringList = [];

            await Task.Run(() =>
            {
                copyFileInformationCopyStringList.Add(ResourceService.GetLocalized("Dialog/FileName") + FileName);
                copyFileInformationCopyStringList.Add(ResourceService.GetLocalized("Dialog/FilePath") + FilePath);
                copyFileInformationCopyStringList.Add(ResourceService.GetLocalized("Dialog/FileSize") + FileSize);
                copyFileInformationCopyStringList.Add(ResourceService.GetLocalized("Dialog/FileSHA256") + FileSHA256);
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyFileInformationCopyStringList));
            sender.Hide();
            await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.FileInformation, copyResult));
        }
    }
}
