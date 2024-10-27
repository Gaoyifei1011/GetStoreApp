using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WinRT;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 本机互操作接口，提供 与 IGraphicsEffect 对应的接口，并允许元数据查询。
    /// </summary>
    [GeneratedComInterface, WindowsRuntimeType, Guid("2FC57384-A068-44D7-A331-30982FCF7177")]
    public partial interface IGraphicsEffectD2D1Interop
    {
        /// <summary>
        /// 检索效果的 ID。
        /// </summary>
        /// <param name="id">此方法返回时，此参数将包含效果的 ID。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int GetEffectId(out Guid id);

        /// <summary>
        /// 检索效果属性的映射。
        /// </summary>
        /// <param name="name">属性的名称。</param>
        /// <param name="index">此方法返回时，此参数将包含 属性的索引。</param>
        /// <param name="mapping">指示强类型效果属性如何映射到基础 Direct2D 效果属性。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int GetNamedPropertyMapping([MarshalAs(UnmanagedType.LPWStr)] string name, out uint index, out GRAPHICS_EFFECT_PROPERTY_MAPPING mapping);

        /// <summary>
        /// 检索效果的属性计数。
        /// </summary>
        /// <param name="count">当方法返回时，此参数将包含属性计数。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int GetPropertyCount(out uint count);

        /// <summary>
        /// 检索指定索引处的效果属性。
        /// </summary>
        /// <param name="index">要检索的属性的索引。</param>
        /// <param name="source">此方法返回时，此参数将包含检索到的属性。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int GetProperty(uint index, out IntPtr value);

        /// <summary>
        /// 检索指定索引处的效果源。
        /// </summary>
        /// <param name="index">检索的源的索引。</param>
        /// <param name="source">当方法返回时，此参数将包含指定索引处的源。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int GetSource(uint index, out IntPtr source);

        /// <summary>
        /// 检索效果的源计数。
        /// </summary>
        /// <param name="count">方法返回时，此参数将包含效果的源计数。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int GetSourceCount(out uint count);
    }
}
