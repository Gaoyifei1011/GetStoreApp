using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.Management.Deployment;
using System;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// WinGet 服务
    /// </summary>
    public static class WinGetService
    {
        // 普通版本的GUID（CLSID）值
        private static readonly Guid CLSID_PackageManager = new Guid("C53A4F16-787E-42A4-B304-29EFFB4BF597");

        private static readonly Guid CLSID_InstallOptions = new Guid("1095F097-EB96-453B-B4E6-1613637F3B14");
        private static readonly Guid CLSID_UnInstallOptions = new Guid("E1D9A11E-9F85-4D87-9C17-2B93143ADB8D");
        private static readonly Guid CLSID_FindPackagesOptions = new Guid("572DED96-9C60-4526-8F92-EE7D91D38C1A");
        private static readonly Guid CLSID_PackageMatchFilter = new Guid("D02C9DAF-99DC-429C-B503-4E504E4AB000");
        private static readonly Guid CLSID_CreateCompositePackageCatalogOptions = new Guid("526534B8-7E46-47C8-8416-B1685C327D37");

        // 开发版本的GUID（CLSID）值
        private static readonly Guid CLSID_PackageManager_Dev = new Guid("74CB3139-B7C5-4B9E-9388-E6616DEA288C");

        private static readonly Guid CLSID_InstallOptions_Dev = new Guid("44FE0580-62F7-44D4-9E91-AA9614AB3E86");
        private static readonly Guid CLSID_UnInstallOptions_Dev = new Guid("AA2A5C04-1AD9-46C4-B74F-6B334AD7EB8C");
        private static readonly Guid CLSID_FindPackagesOptions_Dev = new Guid("1BD8FF3A-EC50-4F69-AEEE-DF4C9D3BAA96");
        private static readonly Guid CLSID_PackageMatchFilter_Dev = new Guid("3F85B9F4-487A-4C48-9035-2903F8A6D9E8");
        private static readonly Guid CLSID_CreateCompositePackageCatalogOptions_Dev = new Guid("EE160901-B317-4EA7-9CC6-5355C6D7D8A7");

        // COM接口：IUnknown 接口
        private static readonly Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        private static bool actuallyUseDev = false;

        public static bool IsOfficialVersionExisted { get; private set; } = false;

        public static bool IsDevVersionExisted { get; private set; } = false;

        /// <summary>
        /// 判断 WinGet 服务是否存在
        /// </summary>
        public static unsafe void InitializeService()
        {
            fixed (Guid* CLSID_PackageManager_Ptr = &CLSID_PackageManager, IID_IUnknown_Ptr = &IID_IUnknown)
            {
                Ole32Library.CoCreateInstance(CLSID_PackageManager_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                IsOfficialVersionExisted = obj != IntPtr.Zero;
            }
            fixed (Guid* CLSID_PackageManager_Dev_Ptr = &CLSID_PackageManager_Dev, IID_IUnknown_Ptr = &IID_IUnknown)
            {
                Ole32Library.CoCreateInstance(CLSID_PackageManager_Dev_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                IsDevVersionExisted = obj != IntPtr.Zero;
            }

            if (WinGetConfigService.UseDevVersion)
            {
                if (IsDevVersionExisted) actuallyUseDev = true;
            }
        }

        /// <summary>
        /// 创建包管理器
        /// </summary>
        public static unsafe PackageManager CreatePackageManager()
        {
            if (actuallyUseDev)
            {
                fixed (Guid* CLSID_PackageManager_Dev_Ptr = &CLSID_PackageManager_Dev, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_PackageManager_Dev_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return PackageManager.FromAbi(obj);
                }
            }
            else
            {
                fixed (Guid* CLSID_PackageManager_Ptr = &CLSID_PackageManager, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_PackageManager_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return PackageManager.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建安装选项
        /// </summary>
        public static unsafe InstallOptions CreateInstallOptions()
        {
            if (actuallyUseDev)
            {
                fixed (Guid* CLSID_InstallOptions_Dev_Ptr = &CLSID_InstallOptions_Dev, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_InstallOptions_Dev_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return InstallOptions.FromAbi(obj);
                }
            }
            else
            {
                fixed (Guid* CLSID_InstallOptions_Ptr = &CLSID_InstallOptions, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_InstallOptions_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return InstallOptions.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建卸载选项
        /// </summary>
        public static unsafe UninstallOptions CreateUninstallOptions()
        {
            if (actuallyUseDev)
            {
                fixed (Guid* CLSID_UnInstallOptions_Dev_Ptr = &CLSID_UnInstallOptions_Dev, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_UnInstallOptions_Dev_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return UninstallOptions.FromAbi(obj);
                }
            }
            else
            {
                fixed (Guid* CLSID_UnInstallOptions_Ptr = &CLSID_UnInstallOptions, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_UnInstallOptions_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return UninstallOptions.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建查找包选项
        /// </summary>
        public static unsafe FindPackagesOptions CreateFindPackagesOptions()
        {
            if (actuallyUseDev)
            {
                fixed (Guid* CLSID_FindPackagesOptions_Dev_Ptr = &CLSID_FindPackagesOptions_Dev, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_FindPackagesOptions_Dev_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return FindPackagesOptions.FromAbi(obj);
                }
            }
            else
            {
                fixed (Guid* CLSID_FindPackagesOptions_Ptr = &CLSID_FindPackagesOptions, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_FindPackagesOptions_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return FindPackagesOptions.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建复合包目录选项
        /// </summary>
        public static unsafe CreateCompositePackageCatalogOptions CreateCreateCompositePackageCatalogOptions()
        {
            if (actuallyUseDev)
            {
                fixed (Guid* CLSID_CreateCompositePackageCatalogOptions_Dev_Ptr = &CLSID_CreateCompositePackageCatalogOptions_Dev, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_CreateCompositePackageCatalogOptions_Dev_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return CreateCompositePackageCatalogOptions.FromAbi(obj);
                }
            }
            else
            {
                fixed (Guid* CLSID_CreateCompositePackageCatalogOptions_Ptr = &CLSID_CreateCompositePackageCatalogOptions, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_CreateCompositePackageCatalogOptions_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return CreateCompositePackageCatalogOptions.FromAbi(obj);
                }
            }
        }

        /// <summary>
        /// 创建包查询匹配过滤选项
        /// </summary>
        public static unsafe PackageMatchFilter CreatePacakgeMatchFilter()
        {
            if (actuallyUseDev)
            {
                fixed (Guid* CLSID_PackageMatchFilter_Dev_Ptr = &CLSID_PackageMatchFilter_Dev, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_PackageMatchFilter_Dev_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return PackageMatchFilter.FromAbi(obj);
                }
            }
            else
            {
                fixed (Guid* CLSID_PackageMatchFilter_Ptr = &CLSID_PackageMatchFilter, IID_IUnknown_Ptr = &IID_IUnknown)
                {
                    Ole32Library.CoCreateInstance(CLSID_PackageMatchFilter_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IUnknown_Ptr, out IntPtr obj);
                    return PackageMatchFilter.FromAbi(obj);
                }
            }
        }
    }
}
