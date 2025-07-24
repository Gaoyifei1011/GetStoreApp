using GetStoreAppShellExtension.Helpers.Root;
using GetStoreAppShellExtension.Services.Root;
using GetStoreAppShellExtension.Services.Settings;
using GetStoreAppShellExtension.WindowsAPI.ComTypes;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppShellExtension.Commands
{
    /// <summary>
    /// 根菜单项
    /// </summary>
    [GeneratedComClass, Guid("C098FAB9-A297-5829-B47E-07EBB207E64A")]
    public partial class RootExplorerCommand : IExplorerCommand
    {
        private readonly string RootMenuString = ResourceService.GetLocalized("ShellMenu/RootMenu");

        /// <summary>
        /// 根菜单标题
        /// </summary>
        public int GetTitle(IShellItemArray psiItemArray, out string ppszName)
        {
            ppszName = RootMenuString;
            return 0;
        }

        /// <summary>
        /// 根菜单图标路径
        /// </summary>
        public int GetIcon(IShellItemArray psiItemArray, out string ppszIcon)
        {
            ppszIcon = Path.Combine(InfoHelper.AppInstalledLocation, "GetStoreApp.exe");
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
            pguidCommandName = typeof(RootExplorerCommand).GUID;
            return 0;
        }

        /// <summary>
        /// 根菜单命令状态（根据应用设置来决定是否显示菜单）
        /// </summary>
        public int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
        {
            if (ShellMenuService.GetShellMenuValue() && psiItemArray is not null && psiItemArray.GetCount(out uint count) is 0 && count >= 1 && psiItemArray.GetItemAt(0, out IShellItem shellItem) is 0)
            {
                shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                if (string.Equals(Path.GetFileName(filePath), "AppxManifest.xml", StringComparison.OrdinalIgnoreCase))
                {
                    pCmdState = EXPCMDSTATE.ECS_ENABLED;
                }
                else
                {
                    string extensionName = Path.GetExtension(filePath);
                    pCmdState = string.Equals(extensionName, ".msix", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msixbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                }
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
        public int Invoke(IShellItemArray psiItemArray, nint pbc)
        {
            return 0;
        }

        /// <summary>
        /// 根菜单命令关联标志
        /// </summary>
        public int GetFlags(out EXPCMDFLAGS pFlags)
        {
            pFlags = EXPCMDFLAGS.ECF_HASSUBCOMMANDS;
            return 0;
        }

        /// <summary>
        /// 根菜单子命令遍历
        /// </summary>
        public int EnumSubCommands(out IEnumExplorerCommand ppEnum)
        {
            IExplorerCommand[] subExplorerCommandArray = [new AppInstallCommand(), new AppInstallAdminCommand(), new PSInstallCommand(), new PSInstallAdminCommand(), new PSRegisterCommand()];
            ppEnum = new EnumExplorerCommand(subExplorerCommandArray);
            return 0;
        }
    }
}
