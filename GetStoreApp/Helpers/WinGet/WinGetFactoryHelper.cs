using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.Management.Deployment;
using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Diagnostics;
using WinRT.Interop;

namespace GetStoreApp.Helpers.WinGet
{
    /// <summary>
    /// WinGet 工厂帮助类
    /// </summary>
    public static class WinGetFactoryHelper
    {
        private static readonly Guid CLSID_Server_PackageManager = new("C53A4F16-787E-42A4-B304-29EFFB4BF597");
        private static readonly Guid CLSID_Server_FindPackagesOptions = new("572DED96-9C60-4526-8F92-EE7D91D38C1A");
        private static readonly Guid CLSID_Server_CreateCompositePackageCatalogOptions = new("526534B8-7E46-47C8-8416-B1685C327D37");
        private static readonly Guid CLSID_Server_InstallOptions = new("1095F097-EB96-453B-B4E6-1613637F3B14");
        private static readonly Guid CLSID_Server_UninstallOptions = new("E1D9A11E-9F85-4D87-9C17-2B93143ADB8D");
        private static readonly Guid CLSID_Server_PackageMatchFilter = new("D02C9DAF-99DC-429C-B503-4E504E4AB000");
        private static readonly Guid CLSID_Server_DownloadOptions = new("4CBABE76-7322-4BE4-9CEA-2589A80682DC");
        private static readonly Guid CLSID_Server_RepairOptions = new("0498F441-3097-455F-9CAF-148F28293865");
        private static readonly Guid CLSID_Server_AddPackageCatalogOptions = new("DB9D012D-00D7-47EE-8FB1-606E10AC4F51");
        private static readonly Guid CLSID_Server_RemovePackageCatalogOptions = new("032B1C58-B975-469B-A013-E632B6ECE8D8");

        /// <summary>
        /// 检查 WinGet 是否存在
        /// </summary>
        public static bool IsExisted()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_PackageManager, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreatePackageManager), 1, e);
                return false;
            }
        }

        /// <summary>
        /// 创建 PackageManager
        /// </summary>
        public static PackageManager CreatePackageManager()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_PackageManager, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? PackageManager.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreatePackageManager), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 FindPackagesOptions
        /// </summary>
        public static FindPackagesOptions CreateFindPackagesOptions()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_FindPackagesOptions, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? FindPackagesOptions.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreateFindPackagesOptions), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 CreateCompositePackageCatalogOptions
        /// </summary>
        public static CreateCompositePackageCatalogOptions CreateCreateCompositePackageCatalogOptions()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_CreateCompositePackageCatalogOptions, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? CreateCompositePackageCatalogOptions.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreateCreateCompositePackageCatalogOptions), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 InstallOptions
        /// </summary>
        public static InstallOptions CreateInstallOptions()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_InstallOptions, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? InstallOptions.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreateInstallOptions), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 UninstallOptions
        /// </summary>
        public static UninstallOptions CreateUninstallOptions()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_UninstallOptions, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? UninstallOptions.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreateUninstallOptions), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 PackageMatchFilter
        /// </summary>
        public static PackageMatchFilter CreatePackageMatchFilter()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_PackageMatchFilter, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? PackageMatchFilter.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreatePackageMatchFilter), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 DownloadOptions
        /// </summary>
        public static DownloadOptions CreateDownloadOptions()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_DownloadOptions, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? DownloadOptions.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreateDownloadOptions), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 RepairOptions
        /// </summary>
        public static RepairOptions CreateRepairOptions()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_RepairOptions, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? RepairOptions.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreateRepairOptions), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 AddPackageCatalogOptions
        /// </summary>
        public static AddPackageCatalogOptions CreateAddPackageCatalogOptions()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_AddPackageCatalogOptions, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? AddPackageCatalogOptions.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreateAddPackageCatalogOptions), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 RemovePackageCatalogOptions
        /// </summary>
        public static RemovePackageCatalogOptions CreateRemovePackageCatalogOptions()
        {
            try
            {
                return Ole32Library.CoCreateInstance(CLSID_Server_RemovePackageCatalogOptions, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION, IID.IID_IUnknown, out IntPtr obj) is 0 ? RemovePackageCatalogOptions.FromAbi(obj) : null;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetFactoryHelper), nameof(CreateRemovePackageCatalogOptions), 1, e);
                return null;
            }
        }
    }
}
