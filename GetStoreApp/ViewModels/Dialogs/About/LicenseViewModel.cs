using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.ViewModels.Base;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GetStoreApp.ViewModels.Dialogs.About
{
    public sealed class LicenseViewModel : ViewModelBase
    {
        private string _licenseText;

        public string LicenseText
        {
            get { return _licenseText; }

            set
            {
                _licenseText = value;
                OnPropertyChanged();
            }
        }

        // 初始化许可证信息
        public IRelayCommand LoadedCommand => new RelayCommand(async () =>
        {
            StorageFile LicenseFile = await StorageFile.GetFileFromPathAsync(Path.Combine(AppContext.BaseDirectory, "LICENSE"));

            IBuffer buffer = await FileIO.ReadBufferAsync(LicenseFile);
            DataReader reader = DataReader.FromBuffer(buffer);
            reader.UnicodeEncoding = UnicodeEncoding.Utf8;
            LicenseText = reader.ReadString(buffer.Length);
        });
    }
}
