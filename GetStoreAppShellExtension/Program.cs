using GetStoreAppShellExtension.Commands;
using GetStoreAppShellExtension.Services.Controls.Settings;
using GetStoreAppShellExtension.Services.Root;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WinRT;

namespace GetStoreAppShellExtension
{
    /// <summary>
    /// 获取商店应用 右键菜单扩展
    /// </summary>
    public static class Program
    {
        static Program()
        {
            ComWrappersSupport.InitializeComWrappers();
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
        }

        /// <summary>
        /// 从 DLL 对象处理程序或对象应用程序中检索类对象。
        /// </summary>
        /// <param name="clsid">将关联正确数据和代码的 CLSID。</param>
        /// <param name="riid">对调用方用于与类对象通信的接口标识符的引用。 通常，IID_IClassFactory (OLE 标头中将其定义为 IClassFactory) 的接口标识符。</param>
        /// <param name="ppv">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，*ppv 包含请求的接口指针。 如果发生错误，接口指针为 NULL。</param>
        /// <returns>此函数可以返回标准返回值E_INVALIDARG、E_OUTOFMEMORY和E_UNEXPECTED，以及以下值。</returns>
        [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
        public static unsafe int DllGetClassObject(Guid clsid, Guid riid, IntPtr* ppv)
        {
            if (clsid.Equals(typeof(RootExplorerCommand).GUID))
            {
                ShellMenuClassFactory classFactory = new();
                IntPtr pIUnknown = (IntPtr)ComInterfaceMarshaller<ShellMenuClassFactory>.ConvertToUnmanaged(classFactory);

                int hresult = StrategyBasedComWrappers.DefaultIUnknownStrategy.QueryInterface((void*)pIUnknown, in riid, out void* ppObj);
                *ppv = (IntPtr)ppObj;
                StrategyBasedComWrappers.DefaultIUnknownStrategy.Release((void*)pIUnknown);

                return hresult;
            }

            return unchecked((int)0x80040111);
        }
    }
}
