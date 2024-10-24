using GetStoreAppInstaller.WindowsAPI.ComTypes;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.GetStoreAppInstaller.WindowsAPI.ComTypes
{
    public static class IGraphicsEffectD2D1InteropMethods
    {
        public static Guid IID { get; } = typeof(IGraphicsEffectD2D1Interop).GUID;

        public static IntPtr AbiToProjectionVftablePtr { get; } = IGraphicsEffectD2D1Interop.Vftbl.InitVtbl();
    }
}
