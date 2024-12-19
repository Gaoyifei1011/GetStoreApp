using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// LUID 结构是不透明的结构，它指定保证在本地计算机上唯一的标识符。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LUID
    {
        public int lowPart;

        public int highPart;
    }
}
