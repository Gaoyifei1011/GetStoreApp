namespace GetStoreApp.Helpers.Controls.Home
{
    public static class GenerateContentHelper
    {
        /// <summary>
        /// 生成要请求的content内容
        /// </summary>
        public static string GenerateRequestContent(string type, string url, string ring, string region)
        {
            return string.Format("type={0}&url={1}&ring={2}&lang={3}", type, url, ring, region);
        }
    }
}
