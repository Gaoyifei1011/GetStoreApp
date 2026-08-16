using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
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
    internal sealed partial class FileInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string FileNameString = ResourceService.GetLocalized("Dialog/FileName");
        private readonly string FilePathString = ResourceService.GetLocalized("Dialog/FilePath");
        private readonly string FileSHA256String = ResourceService.GetLocalized("Dialog/FileSHA256");
        private readonly string FileSizeString = ResourceService.GetLocalized("Dialog/FileSize");
        private readonly CompletedModel completed;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isLoadCompleted;

        private bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsLoadCompleted)));
                }
            }
        }

        private string _fileName;

        private string FileName
        {
            get { return _fileName; }

            set
            {
                if (!string.Equals(_fileName, value))
                {
                    _fileName = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileName)));
                }
            }
        }

        private string _filePath;

        private string FilePath
        {
            get { return _filePath; }

            set
            {
                if (!string.Equals(_filePath, value))
                {
                    _filePath = value;
                    PropertyChanged?.Invoke(this, new(nameof(FilePath)));
                }
            }
        }

        private string _fileSize;

        private string FileSize
        {
            get { return _fileSize; }

            set
            {
                if (!Equals(_fileSize, value))
                {
                    _fileSize = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileSize)));
                }
            }
        }

        private string _fileSHA256;

        private string FileSHA256
        {
            get { return _fileSHA256; }

            set
            {
                if (!string.Equals(_fileSHA256, value))
                {
                    _fileSHA256 = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileSHA256)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal FileInformationDialog(CompletedModel completedItem)
        {
            InitializeComponent();
            completed = completedItem;
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 打开内容对话框后发生的事件
        /// </summary>
        private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            IsLoadCompleted = false;
            if (completed is not null)
            {
                FileName = completed.FileName;
                FilePath = completed.FilePath;
                FileSize = VolumeSizeHelper.ConvertVolumeSizeToString(completed.TotalSize);
            }

            FileSHA256 = await Task.Run(async () => await IOHelper.GetFileSHA256Async(FilePath));
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
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();
            List<string> copyFileInformationCopyStringList = await GetFileInformationCopyStringListAsync(completed);

            if (copyFileInformationCopyStringList is not null)
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyFileInformationCopyStringList));
                contentDialogButtonClickDeferral.Complete();
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 获取文件信息要准备复制的字符串内容
        /// </summary>
        private async Task<List<string>> GetFileInformationCopyStringListAsync(CompletedModel completed)
        {
            if (completed is not null)
            {
                return await Task.Run(async () =>
                {
                    List<string> copyFileInformationCopyStringList = [];

                    copyFileInformationCopyStringList.Add(FileNameString + completed.FileName);
                    copyFileInformationCopyStringList.Add(FilePathString + completed.FilePath);
                    copyFileInformationCopyStringList.Add(FileSizeString + VolumeSizeHelper.ConvertVolumeSizeToString(completed.TotalSize));
                    copyFileInformationCopyStringList.Add(FileSHA256String + await IOHelper.GetFileSHA256Async(completed.FilePath));
                    return copyFileInformationCopyStringList;
                });
            }
            else
            {
                return default;
            }
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
