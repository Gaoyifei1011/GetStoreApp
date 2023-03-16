using System.Runtime.InteropServices;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.WinUI
{
    public static class MileXamlLibrary
    {
        private const string MileXaml = "Mile.Xaml.dll";

        [DllImport(MileXaml, CharSet = CharSet.Auto, EntryPoint = "MileXamlGlobalInitialize", SetLastError = false)]
        public static extern void MileXamlGlobalInitialize();

        [DllImport(MileXaml, CharSet = CharSet.Auto, EntryPoint = "MileXamlGlobalUninitialize", SetLastError = false)]
        public static extern void MileXamlGlobalUninitialize();
    }
}
