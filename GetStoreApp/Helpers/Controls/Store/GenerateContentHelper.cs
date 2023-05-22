namespace GetStoreApp.Helpers.Controls.Store
{
    /// <summary>
    /// 请求内容生成辅助类
    /// </summary>
    public static class GenerateContentHelper
    {
        /// <summary>
        /// 生成要请求的内容
        /// </summary>
        public static string GenerateRequestContent(string type, string url, string ring)
        {
            return string.Format("type={0}&url={1}&ring={2}", type, url, ring);
        }
    }
}
