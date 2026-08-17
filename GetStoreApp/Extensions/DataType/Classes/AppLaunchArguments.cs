using GetStoreApp.Extensions.DataType.Enums;
using System.Collections.Generic;

namespace GetStoreApp.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用启动参数
    /// </summary>
    internal class AppLaunchArguments
    {
        internal AppLaunchKind AppLaunchKind { get; set; }

        internal bool IsLaunched { get; set; }

        internal List<string> SubParameters { get; set; }
    }
}
