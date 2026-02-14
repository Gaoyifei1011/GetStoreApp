using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
        /// 获取计算所得的 SHA256 算法加密后的值
        /// </summary>
        public static string ComputeSHA256(string content)
        {
            HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buffer = Encoding.UTF8.GetBytes(content).AsBuffer();
            IBuffer hashBuffer = hashAlgorithmProvider.HashData(buffer);
            return Convert.ToHexString(hashBuffer.ToArray());
        }
    }
}
