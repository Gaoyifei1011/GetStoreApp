namespace GetStoreApp.Services.Home
{
    public class GenerateContentService
    {
        /// <summary>
        /// 生成要请求的content内容
        /// Generate the content to be requested
        /// </summary>
        public string GenerateContent(string type, string url, string ring, string regionCodeName)
        {
            return string.Format("type={0}&url={1}&ring={2}&lang={3}", type, url, ring, regionCodeName);
        }
    }
}
