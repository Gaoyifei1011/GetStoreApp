using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Converters;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.UI.Dialogs.Download
{
    /// <summary>
    /// 文件信息对话框
    /// </summary>
    public sealed partial class FileInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileSize { get; set; }

        public string FileSHA1 { get; set; }

        private bool _fileCheckState = false;

        public bool FileCheckState
        {
            get { return _fileCheckState; }

            set
            {
                _fileCheckState = value;
                OnPropertyChanged();
            }
        }

        private string _checkFileSHA1;

        public string CheckFileSHA1
        {
            get { return _checkFileSHA1; }

            set
            {
                _checkFileSHA1 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FileInformationDialog(CompletedModel completedItem)
        {
            InitializeComponent();
            FileName = completedItem.FileName;
            FilePath = completedItem.FilePath;
            FileSize = StringConverterHelper.DownloadSizeFormat(completedItem.TotalSize);
            FileSHA1 = completedItem.FileSHA1;
            Task.Run(async () =>
            {
                string fileShA1 = await IOHelper.GetFileSHA1Async(FilePath);
                Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    CheckFileSHA1 = fileShA1;
                    FileCheckState = true;
                });
            });
        }

        /// <summary>
        /// 复制文件信息
        /// </summary>
        public void OnCopyFileInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            Task.Run(() =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileName") + FileName);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FilePath") + FilePath);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileSize") + FileSize);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileSHA1") + FileSHA1);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/CheckFileSHA1") + CheckFileSHA1);

                DispatcherQueue.TryEnqueue(() =>
                {
                    CopyPasteHelper.CopyTextToClipBoard(stringBuilder.ToString());
                    sender.Hide();
                    new DataCopyNotification(this, DataCopyKind.FileInformation).Show();
                });
            });
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
