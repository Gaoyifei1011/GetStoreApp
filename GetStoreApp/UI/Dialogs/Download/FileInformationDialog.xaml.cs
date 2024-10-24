﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GetStoreApp.UI.Dialogs.Download
{
    /// <summary>
    /// 文件信息对话框
    /// </summary>
    public sealed partial class FileInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private string FileName { get; set; }

        private string FilePath { get; set; }

        private string FileSize { get; set; }

        private bool _fileCheckState;

        public bool FileCheckState
        {
            get { return _fileCheckState; }

            set
            {
                if (!Equals(_fileCheckState, value))
                {
                    _fileCheckState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileCheckState)));
                }
            }
        }

        private string _fileSHA1;

        public string FileSHA1
        {
            get { return _fileSHA1; }

            set
            {
                if (!Equals(_fileSHA1, value))
                {
                    _fileSHA1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileSHA1)));
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
            Task.Run(async () =>
            {
                string fileShA1 = await IOHelper.GetFileSHA1Async(FilePath);
                DispatcherQueue.TryEnqueue(() =>
                {
                    FileSHA1 = fileShA1;
                    FileCheckState = true;
                });
            });
        }

        /// <summary>
        /// 复制文件信息
        /// </summary>
        private void OnCopyFileInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            Task.Run(() =>
            {
                List<string> copyFileInformationCopyStringList = [];
                copyFileInformationCopyStringList.Add(ResourceService.GetLocalized("Dialog/FileName") + FileName);
                copyFileInformationCopyStringList.Add(ResourceService.GetLocalized("Dialog/FilePath") + FilePath);
                copyFileInformationCopyStringList.Add(ResourceService.GetLocalized("Dialog/FileSize") + FileSize);
                copyFileInformationCopyStringList.Add(ResourceService.GetLocalized("Dialog/FileSHA1") + FileSHA1);

                DispatcherQueue.TryEnqueue(async () =>
                {
                    bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyFileInformationCopyStringList));
                    sender.Hide();
                    await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.FileInformation, copyResult));
                });
            });
        }
    }
}
