using GetStoreApp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Home
{
    /// <summary>
    /// 生成要请求的内容
    /// Generate the content to be requested
    /// </summary>
    public class GenerateContentService
    {
        /// <summary>
        /// 生成要请求的content内容
        /// Generate the content to be requested
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <param name="ring"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GenerateContent(string type, string url, string ring,string regionCodeName)
        {
            return string.Format("type={0}&url={1}&ring={2}&lang={3}", type, url, ring, regionCodeName);
        }
    }
}
