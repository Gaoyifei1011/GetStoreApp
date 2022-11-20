using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 生成唯一键值
    /// </summary>
    public static class UniqueKeyHelper
    {
        public static string GenerateHistoryKey(string typeName, string channelName, string currentLink)
        {
            // 拼接准备要生成唯一的MD5值的内容
            string Content = string.Format("{0} {1} {2}", typeName, channelName, currentLink);

            return CalculateUniqueKey(Content);
        }

        /// <summary>
        /// 计算下载对应的唯一键值（使用文件名称和文件路径生成）
        /// </summary>
        public static string GenerateDownloadKey(string fileName, string filePath)
        {
            string Content = string.Format("{0} {1}", fileName, filePath);

            return CalculateUniqueKey(Content);
        }

        /// <summary>
        /// 计算唯一键值
        /// </summary>
        private static string CalculateUniqueKey(string content)
        {
            HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);

            IBuffer buffHash = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);

            IBuffer hashedBuffer = hashAlgorithmProvider.HashData(buffHash);

            return CryptographicBuffer.EncodeToHexString(hashedBuffer);
        }
    }
}
