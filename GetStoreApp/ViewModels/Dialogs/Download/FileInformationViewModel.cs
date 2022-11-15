using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Converters.Formats;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml.Controls;
using System.Text;

namespace GetStoreApp.ViewModels.Dialogs.Download
{
    public class FileInformationViewModel : ObservableRecipient
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileSize { get; set; }

        public string FileSHA1 { get; set; }

        public bool FileSHA1Load { get; set; } = true;

        private bool _fileCheckState = false;

        public bool FileCheckState
        {
            get { return _fileCheckState; }

            set { SetProperty(ref _fileCheckState, value); }
        }

        private string _checkFileSHA1;

        public string CheckFileSHA1
        {
            get { return _checkFileSHA1; }

            set { SetProperty(ref _checkFileSHA1, value); }
        }

        // 获取文件的SHA1值
        public IRelayCommand LoadedCommand => new RelayCommand(() =>
        {
            CheckFileSHA1 = IOHelper.GetFileSHA1(FilePath);
            FileCheckState = true;
        });

        // 复制文件信息
        public IRelayCommand CopyFileInformationCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/FileName") + FileName);
            stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/FilePath") + FilePath);
            stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/FileSize") + FileSize);
            if (FileSHA1Load)
            {
                stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/FileSHA1") + FileSHA1);
            }
            stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/CheckFileSHA1") + CheckFileSHA1);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            dialog.Hide();

            new FileInformationCopyNotification(true).Show();
        });

        // 关闭窗口
        public IRelayCommand CloseWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });

        public void InitializeFileInformation(CompletedModel completedItem)
        {
            FileName = completedItem.FileName;
            FilePath = completedItem.FilePath;
            FileSize = System.Convert.ToString(new DownloadSizeFormatConverter().Convert(completedItem.TotalSize, null, null, null));
            if (FileSHA1 == "WebDownloadUnknown")
            {
                FileSHA1Load = false;
            }
            else
            {
                FileSHA1 = completedItem.FileSHA1;
            }
        }
    }
}
