using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using System;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GetStoreApp.ViewModels.Dialogs.About
{
    /// <summary>
    /// 应用许可证对话框视图模型
    /// </summary>
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

        /// <summary>
        /// 对话框加载完成后让内容对话框的烟雾层背景（SmokeLayerBackground）覆盖到标题栏中，并初始化许可证信息
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            StorageFile LicenseFile = await StorageFile.GetFileFromPathAsync(string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), @"Assets\LICENSE"));

            IBuffer buffer = await FileIO.ReadBufferAsync(LicenseFile);
            DataReader reader = DataReader.FromBuffer(buffer);
            reader.UnicodeEncoding = UnicodeEncoding.Utf8;
            LicenseText = reader.ReadString(buffer.Length);
        }
    }
}
