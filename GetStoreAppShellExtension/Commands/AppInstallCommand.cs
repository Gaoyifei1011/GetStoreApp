using GetStoreAppShellExtension.Helpers.Root;
using GetStoreAppShellExtension.Services.Root;
using GetStoreAppShellExtension.WindowsAPI.ComTypes;
using GetStoreAppShellExtension.WindowsAPI.PInvoke.Shell32;
using GetStoreAppShellExtension.WindowsAPI.PInvoke.User32;
using System;
using System.IO;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppShellExtension.Commands
{
    /// <summary>
    /// 使用应用安装器安装
    /// </summary>
    [GeneratedComClass]
    public partial class AppInstallCommand : IExplorerCommand
    {
        /// <summary>
        /// 根菜单标题
        /// </summary>
        public int GetTitle(IShellItemArray psiItemArray, out string ppszName)
        {
            ppszName = ResourceService.GetLocalized("ShellMenu/AppInstall");
            return 0;
        }

        /// <summary>
        /// 根菜单图标路径
        /// </summary>
        public int GetIcon(IShellItemArray psiItemArray, out string ppszIcon)
        {
            ppszIcon = Path.Combine(InfoHelper.AppInstalledLocation, "GetStoreAppInstaller.exe");
            return 0;
        }

        /// <summary>
        /// 根菜单工具提示
        /// </summary>
        public int GetToolTip(IShellItemArray psiItemArray, out string ppszInfotip)
        {
            ppszInfotip = string.Empty;
            return unchecked((int)0x80004001);
        }

        /// <summary>
        /// 根菜单命令 ID
        /// </summary>
        public int GetCanonicalName(out Guid pguidCommandName)
        {
            pguidCommandName = typeof(AppInstallCommand).GUID;
            return 0;
        }

        /// <summary>
        /// 根菜单命令状态（根据应用设置来决定是否显示菜单）
        /// </summary>
        public int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
        {
            if (psiItemArray is not null && psiItemArray.GetCount(out uint count) is 0 && count >= 1 && psiItemArray.GetItemAt(0, out IShellItem shellItem) is 0)
            {
                shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                string extensionName = Path.GetExtension(filePath);
                pCmdState = extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            }
            else
            {
                pCmdState = EXPCMDSTATE.ECS_HIDDEN;
            }
            return 0;
        }

        /// <summary>
        /// 根菜单命令响应处理
        /// </summary>
        public int Invoke(IShellItemArray psiItemArray, IntPtr pbc)
        {
            if (psiItemArray is not null && psiItemArray.GetCount(out uint count) is 0 && count >= 1 && psiItemArray.GetItemAt(0, out IShellItem shellItem) is 0)
            {
                shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                Shell32Library.ShellExecute(IntPtr.Zero, "open", Path.Combine(InfoHelper.AppInstalledLocation, "GetStoreAppInstaller.exe"), filePath, Path.GetDirectoryName(filePath), WindowShowStyle.SW_SHOWNORMAL);
            }
            return 0;
        }

        /// <summary>
        /// 根菜单命令关联标志
        /// </summary>
        public int GetFlags(out EXPCMDFLAGS pFlags)
        {
            pFlags = EXPCMDFLAGS.ECF_DEFAULT;
            return 0;
        }

        /// <summary>
        /// 根菜单子命令遍历
        /// </summary>
        public int EnumSubCommands(out IEnumExplorerCommand ppEnum)
        {
            ppEnum = null;
            return 0;
        }
    }
}
