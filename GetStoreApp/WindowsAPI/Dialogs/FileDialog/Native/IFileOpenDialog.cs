using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog.Native
{
    [ComImport]
    [Guid("42f85136-db7e-439c-85f1-e4075d135fc8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileOpenDialog
    {
        [PreserveSig]
        int Show([In] IntPtr hwnd);

        void SetFileTypes();

        void SetFileTypeIndex();

        void GetFileTypeIndex();

        void Advise();

        void Unadvise();

        void SetOptions([In] FILEOPENDIALOGOPTIONS fos);

        FILEOPENDIALOGOPTIONS GetOptions();

        void SetDefaultFolder();

        void SetFolder(IShellItem psi);

        void GetFolder();

        void GetCurrentSelection();

        void SetFileName();

        void GetFileName();

        void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        void SetOkButtonLabel();

        void SetFileNameLabel();

        void GetResult(out IShellItem ppsi);

        void AddPlace();

        void SetDefaultExtension();

        void Close();

        void SetClientGuid();

        void ClearClientData();

        void SetFilter();

        void GetResults();

        void GetSelectedItems();
    }
}
