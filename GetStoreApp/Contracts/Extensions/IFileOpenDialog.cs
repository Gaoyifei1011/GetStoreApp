using GetStoreApp.Extensions.DataType.Enum;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.Contracts.Extensions
{
    [ComImport]
    [Guid("42f85136-db7e-439c-85f1-e4075d135fc8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileOpenDialog
    {
        [PreserveSig]
        int Show([In] IntPtr hwnd);

        void SetFileTypes();

        void SetFileTypeIndex();

        void GetFileTypeIndex();

        void Advise();

        void Unadvise();

        void SetOptions([In] FILEOPENDIALOGOPTIONS fos);

        FILEOPENDIALOGOPTIONS GetOptions(); // not fully defined

        void SetDefaultFolder(); // not fully defined

        void SetFolder(IShellItem psi);

        void GetFolder(); // not fully defined

        void GetCurrentSelection(); // not fully defined

        void SetFileName();  // not fully defined

        void GetFileName();  // not fully defined

        void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        void SetOkButtonLabel(); // not fully defined

        void SetFileNameLabel(); // not fully defined

        void GetResult(out IShellItem ppsi);

        void AddPlace(); // not fully defined

        void SetDefaultExtension(); // not fully defined

        void Close(); // not fully defined

        void SetClientGuid();  // not fully defined

        void ClearClientData();

        void SetFilter(); // not fully defined

        void GetResults(); // not fully defined

        void GetSelectedItems(); // not fully defined
    }
}
