using GetStoreApp.Contracts.Extensions;
using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Extensions.Native;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.UI.Dialogs.CommonDialog
{
    public class FolderSelectDialog
    {
        /// <summary>
        /// 未指定参数时，使用“我的电脑”目录
        /// </summary>
        public const string ComputerPath = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";

        public string Path { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 显示对话框
        /// </summary>
        public bool ShowDialog()
        {
            return ShowDialog(IntPtr.Zero);
        }

        /// <summary>
        /// 使用父窗口的窗口句柄来显示对话框
        /// </summary>
        public bool ShowDialog(IntPtr hwnd)
        {
            IFileOpenDialog dialog = new FileOpenDialog() as IFileOpenDialog;
            try
            {
                if (hwnd == IntPtr.Zero)
                {
                    hwnd = PInvoke.User32.GetForegroundWindow();
                }

                FILEOPENDIALOGOPTIONS option = dialog.GetOptions();

                // 排除“我的电脑”目录和“网络”目录。
                dialog.SetOptions(option | FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS | FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM);

                IShellItem item;
                if (!string.IsNullOrEmpty(Path))
                {
                    item = NativeMethods.SHCreateItemFromParsingName(Path, IntPtr.Zero, typeof(IShellItem).GUID);

                    dialog.SetFolder(item);
                }

                if (!string.IsNullOrEmpty(Title))
                {
                    dialog.SetTitle(Title);
                }

                int hr = dialog.Show(hwnd);
                if (hr == NativeMethods.ERROR_CANCELLED)
                {
                    return false;
                }

                if (hr != NativeMethods.OK)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                dialog.GetResult(out item);
                item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string name);
                Path = name;

                return true;
            }
            finally
            {
                Marshal.FinalReleaseComObject(dialog);
            }
        }
    }
}
