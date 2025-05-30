using GetStoreApp.Services.Root;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 输入输出辅助类
    /// </summary>
    public static class IOHelper
    {
        /// <summary>
        /// 清空缓存文件夹
        /// </summary>
        public static bool CleanFolder(StorageFolder folder)
        {
            try
            {
                if (string.IsNullOrEmpty(folder.Path) || !Directory.Exists(folder.Path))
                {
                    return true;
                }

                // 删除当前文件夹下所有文件
                foreach (string strFile in Directory.GetFiles(folder.Path))
                {
                    File.Delete(strFile);
                }
                // 删除当前文件夹下所有子文件夹(递归)
                foreach (string strDir in Directory.GetDirectories(folder.Path))
                {
                    Directory.Delete(strDir, true);
                }

                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(IOHelper), nameof(CleanFolder), 1, e);
                return false;
            }
        }

        /// <summary>
        /// 获取文件的 SHA256 值
        /// </summary>
        public static async Task<string> GetFileSHA256Async(string filePath)
        {
            try
            {
                HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);

                StorageFile File = await StorageFile.GetFileFromPathAsync(filePath);
                Stream stream = await File.OpenStreamForReadAsync();
                IInputStream inputStream = stream.AsInputStream();
                uint capacity = 100000000;
                Windows.Storage.Streams.Buffer buffer = new(capacity);
                CryptographicHash buffHash = hashAlgorithmProvider.CreateHash();

                while (true)
                {
                    await inputStream.ReadAsync(buffer, capacity, InputStreamOptions.None);
                    if (buffer.Length > 0)
                    {
                        buffHash.Append(buffer);
                    }
                    else
                    {
                        break;
                    }
                }

                string hashText = CryptographicBuffer.EncodeToHexString(buffHash.GetValueAndReset()).ToLower();

                inputStream.Dispose();
                stream.Dispose();

                return hashText;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, nameof(GetStoreApp), nameof(IOHelper), nameof(GetFileSHA256Async), 1, e);
                return string.Empty;
            }
        }
    }
}
