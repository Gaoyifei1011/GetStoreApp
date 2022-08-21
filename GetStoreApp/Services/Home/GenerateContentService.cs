namespace GetStoreApp.Services.Home
{
    public class GenerateContentService
    {
        /// <summary>
        /// 生成要请求的content内容
        /// </summary>
        public string GenerateContent(string type, string url, string ring, string region)
        {
            return string.Format("type={0}&url={1}&ring={2}&lang={3}", type, url, ring, region);
        }
    }
}
