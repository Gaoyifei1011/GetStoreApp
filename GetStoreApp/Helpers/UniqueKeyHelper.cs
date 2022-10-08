using System.Security.Cryptography;
using System.Text;

namespace GetStoreApp.Helpers
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
            MD5 md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(content));

            // 创建一个 Stringbuilder 来收集字节并创建字符串
            StringBuilder str = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
            for (int i = 0; i < data.Length; i++) str.Append(data[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            // 返回十六进制字符串
            return str.ToString();
        }
    }
}
