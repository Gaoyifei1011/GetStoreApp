using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    [ComImport, Guid("0DD79AE2-D156-45D4-9EEB-3B549769E940"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPinnedList3
    {
        int Placeholder1();

        int Placeholder2();

        int Placeholder3();

        int Placeholder4();

        int Placeholder5();

        int Placeholder6();

        int Placeholder7();

        int Placeholder8();

        int Placeholder9();

        int Placeholder10();

        int Placeholder11();

        int Placeholder12();

        int Placeholder13();

        [PreserveSig]
        int Modify(IntPtr unpin, IntPtr pin, PLMC caller);
    }
}
