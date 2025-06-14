using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 文件信息对话框
    /// </summary>
    public sealed partial class FileInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string FileNameString = ResourceService.GetLocalized("Dialog/FileName");
        private readonly string FilePathString = ResourceService.GetLocalized("Dialog/FilePath");
        private readonly string FileSHA256String = ResourceService.GetLocalized("Dialog/FileSHA256");
        private readonly string FileSizeString = ResourceService.GetLocalized("Dialog/FileSize");

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
                if (!string.Equals(_fileSHA256, value))
                {
                    _fileSHA256 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileSHA256)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FileInformationDialog(CompletedModel completed)
        {
            InitializeComponent();
            FileName = completed.FileName;
            FilePath = completed.FilePath;
            FileSize = VolumeSizeHelper.ConvertVolumeSizeToString(completed.TotalSize);
        }

        /// <summary>
        /// 文件信息对话框初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            string fileShA256 = await Task.Run(async () =>
            {
                return await IOHelper.GetFileSHA256Async(FilePath);
            });

            FileSHA256 = fileShA256;
            IsLoadCompleted = true;
        }

        /// <summary>
        /// 加载完成前禁用关闭对话框
        /// </summary>
        private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
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
                copyFileInformationCopyStringList.Add(FileNameString + FileName);
                copyFileInformationCopyStringList.Add(FilePathString + FilePath);
                copyFileInformationCopyStringList.Add(FileSizeString + FileSize);
                copyFileInformationCopyStringList.Add(FileSHA256String + FileSHA256);
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyFileInformationCopyStringList));
            sender.Hide();
            await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
        }
    }
}
