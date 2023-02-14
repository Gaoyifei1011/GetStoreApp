using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// <see cref="STATSTG"> 结构包含有关开放存储、流或字节数组对象的统计数据。此结构用于 IEnumSTATSTG、ILockBytes、IStorage 和 <see cref="IStream"> 接口。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct STATSTG
    {
        /// <summary>
        /// 指向包含名称的以 NULL 结尾的 Unicode 字符串的指针。此字符串的空间由调用方调用和释放的方法分配（有关详细信息，请参阅 CoTaskMemFree）。若要不返回此成员，请在调用返回 <see cref="STATSTG"> 结构的方法时指定STATFLAG_NONAME值，但对 IEnumSTATSTG::Next 的调用除外，它不提供指定此值的方法。
        /// </summary>
        public string pwcsName;

        /// <summary>
        /// 指示存储对象的类型。这是 STGTY 枚举中的值之一。
        /// </summary>
        public int type;

        /// <summary>
        /// 指定流或字节数组的大小（以字节为单位）。
        /// </summary>
        public long cbSize;

        /// <summary>
        /// 指示此存储、流或字节数组的上次修改时间。
        /// </summary>
        public FILETIME mtime;

        /// <summary>
        /// 指示此存储、流或字节数组的创建时间。
        /// </summary>
        public FILETIME ctime;

        /// <summary>
        /// 指示此存储、流或字节数组的上次访问时间。
        /// </summary>
        public FILETIME atime;

        /// <summary>
        /// 指示打开对象时指定的访问模式。此成员仅在调用 Stat 方法时有效。
        /// </summary>
        public int grfMode;

        /// <summary>
        /// 指示流或字节数组支持的区域锁定类型。
        /// </summary>
        public int grfLocksSupported;

        /// <summary>
        /// 指示存储对象的类标识符;设置为新存储对象的CLSID_NULL。此成员不用于流或字节数组。
        /// </summary>
        public Guid clsid;

        /// <summary>
        /// 指示存储对象的当前状态位;即最近由 IStorage：：SetStateBits 方法设置的值。此成员对流或字节数组无效。
        /// </summary>
        public int grfStateBits;

        /// <summary>
        /// 保留供将来使用。
        /// </summary>
        public int reserved;
    }
}
