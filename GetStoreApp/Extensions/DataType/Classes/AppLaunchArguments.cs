using GetStoreApp.Extensions.DataType.Enums;
using System.Collections.Generic;

namespace GetStoreApp.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用启动参数
    /// </summary>
    public class AppLaunchArguments
    {
        public AppLaunchKind AppLaunchKind { get; set; }

        public bool IsLaunched { get; set; }

        public List<string> SubParameters { get; set; }
    }
}
