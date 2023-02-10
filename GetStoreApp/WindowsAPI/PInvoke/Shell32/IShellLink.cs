using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    [ComImport]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellLink
    {
        [PreserveSig]
        void GetPath(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszFile,
            int cch,
            ref WIN32_FIND_DATA pfd,
            uint fFlags);

        [PreserveSig]
        void GetIDList(out IntPtr ppidl);

        [PreserveSig]
        void SetIDList(IntPtr ppidl);

        [PreserveSig]
        void GetDescription(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszName,
            int cch);

        [PreserveSig]
        void SetDescription(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName);

        [PreserveSig]
        void GetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszDir,
            int cch);

        [PreserveSig]
        void SetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        [PreserveSig]
        void GetArguments(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszArgs,
            int cch);

        [PreserveSig]
        void SetArguments(
            [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        [PreserveSig]
        void GetHotkey(out ushort pwHotkey);

        [PreserveSig]
        void SetHotkey(ushort wHotkey);

        [PreserveSig]
        void GetShowCmd(out int piShowCmd);

        [PreserveSig]
        void SetShowCmd(int iShowCmd);

        [PreserveSig]
        void GetIconLocation(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszIconPath,
            int cch,
            out int piIcon);

        [PreserveSig]
        void SetIconLocation(
            [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
            int iIcon);

        [PreserveSig]
        void SetRelativePath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
            uint dwReserved);

        [PreserveSig]
        void Resolve(
            IntPtr hwnd,
            uint fFlags);

        [PreserveSig]
        void SetPath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
}
