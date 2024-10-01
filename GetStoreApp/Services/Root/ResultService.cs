using GetStoreApp.Extensions.DataType.Enums;
using System;
using Windows.Storage;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 存储运行结果服务
    /// </summary>
    public static class ResultService
    {
        private const string result = "Result";

        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer resultContainer;

        /// <summary>
        /// 初始化结果记录存储服务
        /// </summary>
        public static void Initialize()
        {
            resultContainer = localSettingsContainer.CreateContainer(result, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 获取存储的数据类型
        /// </summary>
        public static StorageDataKind GetStorageDataKind()
        {
            return resultContainer.Values[nameof(StorageDataKind)] is not null ? Enum.TryParse(resultContainer.Values[nameof(StorageDataKind)] as string, out StorageDataKind dataKind) ? dataKind : StorageDataKind.None : StorageDataKind.None;
        }

        /// <summary>
        /// 读取结果存储信息
        /// </summary>
        public static string ReadResult(StorageDataKind dataKind)
        {
            return resultContainer.Values[dataKind.ToString()] is not null ? resultContainer.Values[dataKind.ToString()].ToString() : string.Empty;
        }

        /// <summary>
        /// 保存结果存储信息
        /// </summary>
        public static void SaveResult(StorageDataKind dataKind, string value)
        {
            resultContainer.Values[nameof(StorageDataKind)] = dataKind.ToString();
            resultContainer.Values[dataKind.ToString()] = value;
        }
    }
}
