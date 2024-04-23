using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Winrtact
{
    /// <summary>
    /// Winrtact.dll 函数库
    /// </summary>
    public static partial class WinrtactLibrary
    {
        private const string Winrtact = "winrtact.dll";

        /// <summary>
        /// 提权模式（管理员）下激活 WinGet COM 组件
        /// </summary>
        [LibraryImport(Winrtact, EntryPoint = "WinGetServerManualActivation_CreateInstance", SetLastError = false)]
        public static partial int WinGetServerManualActivation_CreateInstance(ref Guid clsid, ref Guid iid, uint flags, out IntPtr instance);
    }
}
