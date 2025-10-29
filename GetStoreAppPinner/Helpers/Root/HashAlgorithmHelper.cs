using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace GetStoreAppPinner.Helpers.Root
{
    /// <summary>
    /// 哈希算法计算辅助类
    /// </summary>
    public static class HashAlgorithmHelper
    {
        /// <summary>
        /// 拼接并生成唯一的历史记录MD5值
        /// </summary>
        public static string GenerateHistoryKey(string inputContent)
        {
            return ComputeMD5String(inputContent);
        }

        /// <summary>
        /// 拼接并生成唯一的历史记录MD5值
        /// </summary>
        public static string GenerateHistoryKey(string typeName, string channelName, string currentLink)
        {
            return ComputeMD5String(string.Format("{0} {1} {2}", typeName, channelName, currentLink));
        }

        /// <summary>
        /// 计算下载对应的唯一键值（使用文件名称和文件路径生成）
        /// </summary>
        public static string GenerateDownloadKey(string fileName, string filePath)
        {
            return ComputeMD5String(string.Format("{0} {1}", fileName, filePath));
        }

        /// <summary>
        /// 获取计算所得的 MD5 算法加密后的值
        /// </summary>
        private static string ComputeMD5String(string content)
        {
            HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buffHash = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);
            IBuffer hashedBuffer = hashAlgorithmProvider.HashData(buffHash);
            return CryptographicBuffer.EncodeToHexString(hashedBuffer);
        }

        /// <summary>
        /// 获取计算所得的 SHA256 算法加密后的值
        /// </summary>
        public static byte[] ComputeSHA256Hash(string content)
        {
            HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buffHash = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);
            IBuffer hashedBuffer = hashAlgorithmProvider.HashData(buffHash);
            CryptographicBuffer.CopyToByteArray(hashedBuffer, out byte[] hashBytes);
            return hashBytes;
        }
    }
}
