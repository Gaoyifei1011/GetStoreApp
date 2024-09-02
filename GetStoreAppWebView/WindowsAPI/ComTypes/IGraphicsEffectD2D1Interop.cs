using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Graphics.Effects;
using WinRT;
using WinRT.Interop;

namespace GetStoreAppWebView.WindowsAPI.ComTypes
{
    /// <summary>
    /// 本机互操作接口，提供 与 IGraphicsEffect 对应的接口，并允许元数据查询。
    /// 不支持 COM 源生成
    /// </summary>
    [WindowsRuntimeType, Guid("2FC57384-A068-44D7-A331-30982FCF7177")]
    public interface IGraphicsEffectD2D1Interop
    {
        /// <summary>
        /// 检索效果的 ID。
        /// </summary>
        /// <param name="id">此方法返回时，此参数将包含效果的 ID。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        int GetEffectId(out Guid id);

        /// <summary>
        /// 检索效果属性的映射。
        /// </summary>
        /// <param name="name">属性的名称。</param>
        /// <param name="index">此方法返回时，此参数将包含 属性的索引。</param>
        /// <param name="mapping">指示强类型效果属性如何映射到基础 Direct2D 效果属性。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        int GetNamedPropertyMapping(IntPtr name, out uint index, out GRAPHICS_EFFECT_PROPERTY_MAPPING mapping);

        /// <summary>
        /// 检索效果的属性计数。
        /// </summary>
        /// <param name="count">当方法返回时，此参数将包含属性计数。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        int GetPropertyCount(out uint count);

        /// <summary>
        /// 检索指定索引处的效果属性。
        /// </summary>
        /// <param name="index">要检索的属性的索引。</param>
        /// <param name="source">此方法返回时，此参数将包含检索到的属性。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        int GetProperty(uint index, out IntPtr value);

        /// <summary>
        /// 检索指定索引处的效果源。
        /// </summary>
        /// <param name="index">检索的源的索引。</param>
        /// <param name="source">当方法返回时，此参数将包含指定索引处的源。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        int GetSource(uint index, out IGraphicsEffectSource source);

        /// <summary>
        /// 检索效果的源计数。
        /// </summary>
        /// <param name="count">方法返回时，此参数将包含效果的源计数。</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        int GetSourceCount(out uint count);

        internal unsafe struct Vftbl
        {
            public static IntPtr InitVtbl()
            {
                Vftbl* lpVtbl = (Vftbl*)ComWrappersSupport.AllocateVtableMemory(typeof(Vftbl), sizeof(Vftbl));

                lpVtbl->IUnknownVftbl = IUnknownVftbl.AbiToProjectionVftbl;
                lpVtbl->GetEffectId = &GetEffectIdFromAbi;
                lpVtbl->GetNamedPropertyMapping = &GetNamedPropertyMappingFromAbi;
                lpVtbl->GetPropertyCount = &GetPropertyCountFromAbi;
                lpVtbl->GetProperty = &GetPropertyFromAbi;
                lpVtbl->GetSource = &GetSourceFromAbi;
                lpVtbl->GetSourceCount = &GetSourceCountFromAbi;
                return (IntPtr)lpVtbl;
            }

            private IUnknownVftbl IUnknownVftbl;

            // interface delegates
            private delegate* unmanaged[MemberFunction]<IntPtr, Guid*, int> GetEffectId;

            private delegate* unmanaged[MemberFunction]<IntPtr, IntPtr, uint*, GRAPHICS_EFFECT_PROPERTY_MAPPING*, int> GetNamedPropertyMapping;
            private delegate* unmanaged[MemberFunction]<IntPtr, uint*, int> GetPropertyCount;
            private delegate* unmanaged[MemberFunction]<IntPtr, uint, IntPtr*, int> GetProperty;
            private delegate* unmanaged[MemberFunction]<IntPtr, uint, IntPtr*, int> GetSource;
            private delegate* unmanaged[MemberFunction]<IntPtr, uint*, int> GetSourceCount;

            // interface implementation
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetEffectIdFromAbi(IntPtr thisPtr, Guid* value)
            {
                try
                {
                    if (value != null)
                    {
                        *value = Guid.Empty;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetEffectId(out Guid v);
                    if (hr >= 0)
                    {
                        if (value != null)
                        {
                            *value = v;
                        }
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return Marshal.GetHRForException(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetNamedPropertyMappingFromAbi(IntPtr thisPtr, IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping)
            {
                try
                {
                    if (index != null)
                    {
                        *index = 0;
                    }

                    if (mapping != null)
                    {
                        *mapping = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetNamedPropertyMapping(name, out uint i, out GRAPHICS_EFFECT_PROPERTY_MAPPING m);
                    if (hr >= 0)
                    {
                        if (index != null)
                        {
                            *index = i;
                        }

                        if (mapping != null)
                        {
                            *mapping = m;
                        }
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return Marshal.GetHRForException(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetPropertyCountFromAbi(IntPtr thisPtr, uint* value)
            {
                try
                {
                    if (value != null)
                    {
                        *value = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetPropertyCount(out uint v);
                    if (hr >= 0)
                    {
                        *value = v;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return Marshal.GetHRForException(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetPropertyFromAbi(IntPtr thisPtr, uint index, IntPtr* value)
            {
                try
                {
                    if (value != null)
                    {
                        *value = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetProperty(index, out IntPtr v);
                    if (hr >= 0)
                    {
                        *value = v;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return Marshal.GetHRForException(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetSourceFromAbi(IntPtr thisPtr, uint index, IntPtr* value)
            {
                try
                {
                    if (value != null)
                    {
                        *value = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetSource(index, out IGraphicsEffectSource v);
                    if (hr >= 0)
                    {
                        IntPtr unk = MarshalInspectable<IGraphicsEffectSource>.FromManaged(v!);
                        *value = unk;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return Marshal.GetHRForException(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetSourceCountFromAbi(IntPtr thisPtr, uint* value)
            {
                try
                {
                    if (value != null)
                    {
                        *value = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetSourceCount(out uint v);
                    if (hr >= 0)
                    {
                        *value = v;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return Marshal.GetHRForException(e);
                }
            }
        }
    }
}
