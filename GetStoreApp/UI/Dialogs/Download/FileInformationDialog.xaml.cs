using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
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
        private string FileName;
        private string FilePath;
        private string FileSize;

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

        private string _fileSHA1;

        public string FileSHA1
        {
            get { return _fileSHA1; }

            set
            {
                _fileSHA1 = value;
                OnPropertyChanged();
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
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileName") + FileName);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FilePath") + FilePath);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileSize") + FileSize);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/FileSHA1") + FileSHA1);

                DispatcherQueue.TryEnqueue(() =>
                {
                    bool copyResult = CopyPasteHelper.CopyTextToClipBoard(stringBuilder.ToString());
                    sender.Hide();
                    TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.FileInformation, copyResult));
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
