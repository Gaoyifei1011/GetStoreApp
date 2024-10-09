using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Graphics.Effects;
using WinRT;
using WinRT.Interop;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 本机互操作接口，提供 与 IGraphicsEffect 对应的接口，并允许元数据查询。
    /// 暂不支持 COM 源生成
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
        int GetNamedPropertyMapping(string name, out uint index, out GRAPHICS_EFFECT_PROPERTY_MAPPING mapping);

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

            // 接口委托

            private delegate* unmanaged[MemberFunction]<IntPtr, Guid*, int> GetEffectId;
            private delegate* unmanaged[MemberFunction]<IntPtr, IntPtr, uint*, GRAPHICS_EFFECT_PROPERTY_MAPPING*, int> GetNamedPropertyMapping;
            private delegate* unmanaged[MemberFunction]<IntPtr, uint*, int> GetPropertyCount;
            private delegate* unmanaged[MemberFunction]<IntPtr, uint, IntPtr*, int> GetProperty;
            private delegate* unmanaged[MemberFunction]<IntPtr, uint, IntPtr*, int> GetSource;
            private delegate* unmanaged[MemberFunction]<IntPtr, uint*, int> GetSourceCount;

            // 接口实现

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetEffectIdFromAbi(IntPtr thisPtr, Guid* id)
            {
                try
                {
                    if (id is not null)
                    {
                        *id = Guid.Empty;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetEffectId(out Guid idObject);
                    if (hr >= 0)
                    {
                        *id = idObject;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetNamedPropertyMappingFromAbi(IntPtr thisPtr, IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping)
            {
                try
                {
                    if (index is not null)
                    {
                        *index = 0;
                    }

                    if (mapping is not null)
                    {
                        *mapping = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetNamedPropertyMapping(Utf16StringMarshaller.ConvertToManaged((ushort*)name), out uint indexObject, out GRAPHICS_EFFECT_PROPERTY_MAPPING mappingObject);
                    if (hr >= 0)
                    {
                        *index = indexObject;
                        *mapping = mappingObject;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetPropertyCountFromAbi(IntPtr thisPtr, uint* count)
            {
                try
                {
                    if (count is not null)
                    {
                        *count = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetPropertyCount(out uint countObject);
                    if (hr >= 0)
                    {
                        *count = countObject;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetPropertyFromAbi(IntPtr thisPtr, uint index, IntPtr* value)
            {
                try
                {
                    if (value is not null)
                    {
                        *value = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetProperty(index, out IntPtr valueObject);
                    if (hr >= 0)
                    {
                        *value = valueObject;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetSourceFromAbi(IntPtr thisPtr, uint index, IntPtr* source)
            {
                try
                {
                    if (source is not null)
                    {
                        *source = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetSource(index, out IGraphicsEffectSource sourceObject);
                    if (hr >= 0)
                    {
                        *source = MarshalInspectable<IGraphicsEffectSource>.FromManaged(sourceObject!);
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
            private static int GetSourceCountFromAbi(IntPtr thisPtr, uint* count)
            {
                try
                {
                    if (count is not null)
                    {
                        *count = 0;
                    }

                    int hr = ComWrappersSupport.FindObject<IGraphicsEffectD2D1Interop>(thisPtr).GetSourceCount(out uint countObject);
                    if (hr >= 0)
                    {
                        *count = countObject;
                    }
                    return hr;
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);
                    return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
                }
            }
        }
    }
}
