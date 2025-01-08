using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 创建可在属性存储中存储的 IPropertyValue 对象。
    /// </summary>
    [GeneratedComInterface, Guid("629BDBC8-D932-4FF4-96B9-8D96C5C1E858")]
    public partial interface IPropertyValueStatics : IInspectable
    {
        /// <summary>
        /// 创建表示空值的新 IPropertyValue 对象。
        /// </summary>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateEmpty(out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 8 位布尔值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateUInt8(byte value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的有符号 16 位整数值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateInt16(short value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的无符号 16 位整数值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateUInt16(ushort value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的有符号 32 位整数值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateInt32(int value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的无符号 32 位整数值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateUInt32(uint value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的有符号 64 位整数值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateInt64(long value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的无符号 64 位整数值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateUInt64(ulong value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 32 位浮点值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateSingle(float value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 64 位浮点值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateDouble(double value, out IntPtr propertyValue);

        /// <summary>
        /// 创建包含指定 Unicode 字符的新 IPropertyValue 对象。
        /// </summary>
        /// <param name="value">要存储的 Unicode 字符。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateChar16(char value, out IntPtr propertyValue);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateBoolean([MarshalAs(UnmanagedType.Bool)] bool value, out IntPtr propertyValue);

        /// <summary>
        /// 创建包含指定字符串值的新 IPropertyValue 对象。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateString(IntPtr value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 IInspectable 对象。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateInspectable(IntPtr value, out IntPtr propertyValue);

        /// <summary>
        /// 创建包含指定 GUID 值的新 IPropertyValue 对象。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateGuid(Guid value, out IntPtr propertyValue);

        /// <summary>
        /// 创建包含指定 DateTime 值的新 IPropertyValue 对象。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateDateTime(DateTimeOffset value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 TimeSpan 值。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateTimeSpan(TimeSpan value, out IntPtr propertyValue);

        /// <summary>
        /// 创建包含指定 Point 值的新 IPropertyValue 对象。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreatePoint(Point value, out IntPtr propertyValue);

        /// <summary>
        /// 创建包含指定 Size 值的新 IPropertyValue 对象。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateSize(Size value, out IntPtr propertyValue);

        /// <summary>
        /// 创建包含指定 Rect 值的新 IPropertyValue 对象。
        /// </summary>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateRect(Rect value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的无符号 8 位整数值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateUInt8Array(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的有符号 16 位整数值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateInt16Array(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] short[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的无符号 16 位整数值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateUInt16Array(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ushort[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的有符号 32 位整数值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateInt32Array(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的无符号 32 位整数值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateUInt32Array(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] uint[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的有符号 64 位整数值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateInt64Array(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] long[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的无符号 64 位整数值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateUInt64Array(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ulong[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 32 位浮点值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateSingleArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] float[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 64 位浮点值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateDoubleArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 Unicode 字符数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的 Unicode 字符数组。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateChar16Array(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] char[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 8 位布尔值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateBooleanArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = UnmanagedType.Bool)] bool[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的字符串值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateStringArray(int valueLength, IntPtr value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 IInspectable 对象数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateInspectableArray(int valueLength, IntPtr value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 Guid 值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateGuidArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Guid[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 DateTime 值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateDateTimeArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] DateTimeOffset[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 TimeSpan 值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateTimeSpanArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] TimeSpan[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 Point 值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreatePointArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Point[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 Size 值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateSizeArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Size[] value, out IntPtr propertyValue);

        /// <summary>
        /// 创建一个新的 IPropertyValue 对象，该对象包含指定的 Rect 值数组。
        /// </summary>
        /// <param name="valueLength">数组长度</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="propertyValue">指向新对象的指针，该对象将其 Type 属性设置为 PropertyType_Empty。 新对象中不存储任何值。 使用 IUnknown::QueryInterface 方法获取对象的 IPropertyValue 接口。</param>
        /// <returns>此方法可以返回其中一个值。S_OK 为已成功创建属性值，E_POINTER 为 NULL，E_OUTOFMEMORY 为无法创建 IPropertyValue 对象。</returns>
        [PreserveSig]
        int CreateRectArray(int valueLength, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Rect[] value, out IntPtr propertyValue);
    }
}
