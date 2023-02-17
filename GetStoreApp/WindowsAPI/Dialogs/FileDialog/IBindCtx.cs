using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 提供对绑定上下文的访问，该上下文是一个对象，用于存储有关特定名字对象绑定操作的信息。
    /// </summary>
    [Guid("0000000e-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IBindCtx
    {
        /// <summary>
        /// 向绑定上下文注册对象，以确保该对象在释放绑定上下文之前保持活动状态。
        /// </summary>
        /// <param name="punk">指向要注册为绑定的对象上的 IUnknown 接口的指针。</param>
        void RegisterObjectBound([MarshalAs(UnmanagedType.Interface)] object punk);

        /// <summary>
        /// 从绑定上下文中删除对象，撤消对 <see cref="RegisterObjectBound"> 的上一次调用。
        /// </summary>
        /// <param name="punk">指向要删除的对象上的 IUnknown 接口的指针。</param>
        void RevokeObjectBound([MarshalAs(UnmanagedType.Interface)] object punk);

        /// <summary>
        /// 释放以前通过对 <see cref="RegisterObjectBound"> 进行注册的所有对象的指针。
        /// </summary>
        void ReleaseBoundObjects();

        /// <summary>
        /// 为绑定上下文中存储的绑定参数设置新值。
        /// </summary>
        /// <param name="pbindopts">指向包含绑定参数的 <see cref="BIND_OPTS"> 结构的指针。</param>
        void SetBindOptions([In()] ref BIND_OPTS pbindopts);

        /// <summary>
        /// 检索此绑定上下文中存储的绑定选项。
        /// </summary>
        /// <param name="pbindopts">指向返回时接收当前绑定参数的已初始化结构的指针。 请参阅 <see cref="BIND_OPTS">。</param>
        void GetBindOptions(ref BIND_OPTS pbindopts);

        /// <summary>
        /// 检索指向正在运行的对象表的接口指针， (ROT) 运行此绑定上下文的计算机。
        /// </summary>
        /// <param name="pprot"><see cref="IRunningObjectTable"> 指针变量的地址，该变量接收指向正在运行的对象表的接口指针。 如果发生错误，<param name="pprot"> 设置为 NULL。 如果 <param name="pprot"> 为非 NULL，则实现会在正在运行的表对象上调用 AddRef ;调用方负责调用 Release。</param>
        void GetRunningObjectTable(out IRunningObjectTable pprot);

        /// <summary>
        /// 将对象与绑定上下文的字符串键指针表中的字符串键相关联。
        /// </summary>
        /// <param name="pszKey">要在其中注册对象的 绑定上下文字符串键 。 键字符串比较区分大小写。</param>
        /// <param name="punk">指向要注册的对象上的 IUnknown 接口的指针。</param>
        void RegisterObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey, [MarshalAs(UnmanagedType.Interface)] object punk);

        /// <summary>
        /// 检索指向绑定上下文的字符串键指针表中与指定键关联的对象的接口指针。
        /// </summary>
        /// <param name="pszKey">要搜索的绑定上下文字符串键。键字符串比较区分大小写。</param>
        /// <param name="ppunk">接收指向与 <param name="pszKey"> 关联的对象的接口指针的 IUnknown* 指针变量的地址。成功后，实现会在 <param name="ppunk"> 上调用 AddRef。调用方有责任调用“发布”。如果发生错误，实现会将 <param name="ppunk"> 设置为 NULL。</param>
        void GetObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey, [MarshalAs(UnmanagedType.Interface)] out object ppunk);

        /// <summary>
        /// 检索指向接口的指针，该接口可用于枚举绑定上下文字符串键的指针表的键。
        /// </summary>
        /// <param name="ppenum">接收指向枚举器的接口指针的 <see cref="IEnumString"> 指针变量的地址。 如果发生错误，<param name="ppenum"> 设置为 NULL。 如果 <param name="ppenum"> 为非 NULL， 则实现调用 <param name="ppenum"> 上的 AddRef;调用方负责调用发布。</param>
        void EnumObjectParam(out IEnumString ppenum);

        /// <summary>
        /// 从绑定上下文的字符串键化对象表中删除指定的键及其关联的指针。 该键以前必须已插入到表中，并调用 <see cref="RegisterObjectParam">。
        /// </summary>
        /// <param name="pszKey">要删除的 绑定上下文字符串键 。 键字符串比较区分大小写。</param>
        /// <returns></returns>
        [PreserveSig]
        int RevokeObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey);
    }
}
