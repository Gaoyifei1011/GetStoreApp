using System;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 哈希算法计算辅助类
    /// </summary>
    internal static class HashAlgorithmHelper
    {
        /// <summary>
        /// 拼接并生成唯一的历史记录MD5值
        /// </summary>
        internal static string GenerateHistoryKey(string inputContent)
        {
            if (string.IsNullOrEmpty(inputContent))
            {
                return string.Empty;
            }

            return ComputeMD5(inputContent);
        }

        /// <summary>
        /// 拼接并生成唯一的历史记录MD5值
        /// </summary>
        internal static string GenerateHistoryKey(string typeName, string channelName, string currentLink)
        {
            if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(channelName) || string.IsNullOrEmpty(currentLink))
            {
                return string.Empty;
            }

            return ComputeMD5(string.Format("{0} {1} {2}", typeName, channelName, currentLink));
        }

        /// <summary>
        /// 计算下载对应的唯一键值（使用文件名称和文件路径生成）
        /// </summary>
        internal static string GenerateDownloadKey(string fileName, string filePath)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            return ComputeMD5(string.Format("{0} {1}", fileName, filePath));
        }

        /// <summary>
        /// 获取计算所得的 SHA256 算法加密后的值
        /// </summary>
        internal static string ComputeSHA256(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }

            HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buffHash = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);
            IBuffer hashedBuffer = hashAlgorithmProvider.HashData(buffHash);
            CryptographicBuffer.CopyToByteArray(hashedBuffer, out byte[] hashBytes);
            byte[] tokenBytes = new byte[16];
            Array.Copy(hashBytes, tokenBytes, tokenBytes.Length);
            return Convert.ToBase64String(tokenBytes);
        }

        /// <summary>
        /// 获取计算所得的 MD5 算法加密后的值
        /// </summary>
        private static string ComputeMD5(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }

            HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buffHash = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);
            IBuffer hashedBuffer = hashAlgorithmProvider.HashData(buffHash);
            return CryptographicBuffer.EncodeToHexString(hashedBuffer);
        }
    }
}
