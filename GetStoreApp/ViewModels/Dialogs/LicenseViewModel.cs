using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class LicenseViewModel : ObservableRecipient
    {
        public string LicenseText { get; set; }

        public async void InitializeText()
        {
            StorageFile LicenseFile = await StorageFile.GetFileFromPathAsync(Path.Combine(AppContext.BaseDirectory, "LICENSE"));

            IBuffer buffer = await FileIO.ReadBufferAsync(LicenseFile);
            DataReader reader = DataReader.FromBuffer(buffer);
            reader.UnicodeEncoding = UnicodeEncoding.Utf8;
            LicenseText = reader.ReadString(buffer.Length);
        }
    }
}
