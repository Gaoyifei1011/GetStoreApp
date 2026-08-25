using System;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace GetStoreAppPinner.Helpers.Root
{
    /// <summary>
    /// 哈希算法计算辅助类
    /// </summary>
    internal static class HashAlgorithmHelper
    {
        /// <summary>
        /// 获取计算所得的 SHA256 算法加密后的值
        /// </summary>
        internal static string ComputeSHA256(string content)
        {
            HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buffHash = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);
            IBuffer hashedBuffer = hashAlgorithmProvider.HashData(buffHash);
            CryptographicBuffer.CopyToByteArray(hashedBuffer, out byte[] hashBytes);
            byte[] tokenBytes = new byte[16];
            Array.Copy(hashBytes, tokenBytes, tokenBytes.Length);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}
