using System;
using Windows.ApplicationModel;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsMSIX { get; }

        /// <summary>
        /// 判断应用是否在 Msix 容器中运行
        /// </summary>
        static RuntimeHelper()
        {
            try
            {
                Package currentPackage = Package.Current;
                if (currentPackage is not null)
                {
                    IsMSIX = true;
                }
                else
                {
                    IsMSIX = false;
                }
            }
            catch (Exception)
            {
                IsMSIX = false;
            }
        }
    }
}
