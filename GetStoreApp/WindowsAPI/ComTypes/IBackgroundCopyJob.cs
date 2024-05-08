using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 使用 IBackgroundCopyJob 接口将文件添加到作业、设置作业的优先级、确定作业的状态，以及启动和停止作业。
    /// 若要创建作业，请调用 IBackgroundCopyManager：：CreateJob 方法。 若要获取指向现有作业 的 IBackgroundCopyJob 接口指针，请调用 IBackgroundCopyManager：：GetJob 方法。
    /// </summary>
    [GeneratedComInterface, Guid("37668D37-507E-4160-9316-26306D150B12")]
    public partial interface IBackgroundCopyJob
    {
        /// <summary>
        /// 将多个文件添加到作业。
        /// </summary>
        /// <param name="cFileCount">paFileSet 中的元素数。</param>
        /// <param name="pFileSet">
        /// BG_FILE_INFO结构的数组，用于标识要传输的文件的本地和远程文件名。
        /// 上传作业仅限于单个文件。 如果数组包含多个元素，或者作业已包含文件，则 方法返回BG_E_TOO_MANY_FILES。
        /// </param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int AddFileSet(uint cFileCount, IntPtr pFileSet);

        /// <summary>
        /// 将单个文件添加到作业。
        /// </summary>
        /// <param name="RemoteUrl">以 Null 结尾的字符串，其中包含服务器上文件的名称。 有关指定远程名称的信息，请参阅BG_FILE_INFO结构的 RemoteName 成员和备注部分。</param>
        /// <param name="LocalName">以 Null 结尾的字符串，其中包含客户端上文件的名称。 有关指定本地名称的信息，请参阅 BG_FILE_INFO 结构的 LocalName 成员和备注部分。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int AddFile([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName);

        /// <summary>
        /// 检索用于枚举作业中的文件的IEnumBackgroundCopyFiles 接口指针。
        /// </summary>
        /// <param name="pEnum">用于枚举作业中的文件的 IEnumBackgroundCopyFiles 接口指针。 完成后释放 ppEnumFiles 。</param>
        /// <returns>此方法在成功时返回 S_OK 或错误时返回标准 COM HRESULT 值之一。</returns>
        [PreserveSig]
        int EnumFiles(out IntPtr pEnum);

        /// <summary>
        /// 挂起作业。 新作业、出错的作业和已完成文件传输的作业将自动挂起。
        /// </summary>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int Suspend();

        /// <summary>
        /// 激活新作业或重启已暂停的作业。
        /// </summary>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int Resume();

        /// <summary>
        /// 从传输队列中删除作业，并从客户端中删除相关的临时文件， (下载) 和服务器 (上传) 。
        /// </summary>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int Cancel();

        /// <summary>
        /// 结束作业并将传输的文件保存在客户端上。
        /// </summary>
        /// <returns>此方法返回以下 HRESULT 值。 方法还可以返回与将传输的文件的临时副本重命名为其给定名称相关的错误。</returns>
        [PreserveSig]
        int Complete();

        /// <summary>
        /// 检索用于标识队列中作业的标识符。
        /// </summary>
        /// <param name="pVal">标识 BITS 队列中的作业的 GUID。</param>
        /// <returns>此方法在成功时返回 S_OK 或错误时返回标准 COM HRESULT 值之一。</returns>
        [PreserveSig]
        int GetId(out Guid pVal);

        /// <summary>
        /// 检索正在执行的传输类型，例如文件下载或上传。
        /// </summary>
        /// <param name="pVal">正在执行的传输类型。 有关传输类型的列表，请参阅 BG_JOB_TYPE 枚举。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetType(out BG_JOB_TYPE pVal);

        /// <summary>
        /// 检索与作业相关的进度信息，例如传输的字节数和文件数。
        /// </summary>
        /// <param name="pVal">包含可用于计算完成作业百分比的数据。 有关详细信息，请参阅 BG_JOB_PROGRESS。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetProgress(out BG_JOB_PROGRESS pVal);

        /// <summary>
        /// 检索与作业相关的时间戳，例如作业的创建时间或上次修改时间。
        /// </summary>
        /// <param name="pVal">包含与作业相关的时间戳。 有关可用的时间戳，请参阅 BG_JOB_TIMES 结构。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetTimes(out IntPtr pVal);

        /// <summary>
        /// 检索作业的状态。
        /// </summary>
        /// <param name="pVal">作业的状态。 例如，状态反映作业是出错、传输数据还是挂起。 有关作业状态的列表，请参阅 BG_JOB_STATE 枚举。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetState(out BG_JOB_STATE pVal);

        /// <summary>
        /// 在发生错误后检索错误接口。
        /// 当作业的状态为 BG_JOB_STATE_ERROR或BG_JOB_STATE_TRANSIENT_ERROR 时，BITS 将生成错误对象。 当对 IBackgroundCopyXXXX 接口方法的调用失败时，服务不会创建错误对象。 在 BITS 开始传输数据(作业的状态更改为作业BG_JOB_STATE_TRANSFERRING) 或应用程序退出之前，错误对象才可用。
        /// </summary>
        /// <param name="ppError">错误接口，提供错误代码、错误说明以及发生错误的上下文。 此参数还标识发生错误时正在传输的文件。 完成后释放 ppError 。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError ppError);

        /// <summary>
        /// 检索作业所有者的标识。
        /// </summary>
        /// <param name="pVal">以 Null 结尾的字符串，其中包含标识作业所有者的 SID 的字符串版本。 完成后，调用 CoTaskMemFree 函数以释放 ppOwner 。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

        /// <summary>
        /// 指定作业的显示名称。 通常，使用显示名称在用户界面中标识作业。
        /// </summary>
        /// <param name="Val">标识作业的以 Null 结尾的字符串。 不得为 NULL。 字符串的长度限制为 256 个字符，不包括 null 终止符。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Val);

        /// <summary>
        /// 检索作业的显示名称。 通常，使用显示名称在用户界面中标识作业。
        /// </summary>
        /// <param name="pVal">以 Null 结尾的字符串，其中包含标识作业的显示名称。 多个作业可以具有相同的显示名称。 完成后，调用 CoTaskMemFree 函数以释放 ppDisplayName 。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

        /// <summary>
        /// 提供作业的说明。
        /// </summary>
        /// <param name="Val">以 Null 结尾的字符串，提供有关作业的其他信息。 字符串的长度限制为 1,024 个字符，不包括 null 终止符。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int SetDescription([MarshalAs(UnmanagedType.LPWStr)] string Val);

        /// <summary>
        /// 检索作业的说明。
        /// </summary>
        /// <param name="pVal">以 Null 结尾的字符串，其中包含作业的简短说明。 完成后，调用 CoTaskMemFree 函数以释放 ppDescription 。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

        /// <summary>
        /// 指定作业的优先级。 优先级确定相对于传输队列中的其他作业处理作业的时间。
        /// </summary>
        /// <param name="Val">指定作业相对于传输队列中其他作业的优先级。 默认值为 BG_JOB_PRIORITY_NORMAL。 有关优先级的列表，请参阅 BG_JOB_PRIORITY 枚举。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int SetPriority(BG_JOB_PRIORITY Val);

        /// <summary>
        /// 检索作业的优先级。 优先级确定相对于传输队列中其他作业处理作业的时间。
        /// </summary>
        /// <param name="pVal">作业相对于传输队列中其他作业的优先级。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetPriority(out BG_JOB_PRIORITY pVal);

        /// <summary>
        /// 指定要接收的事件通知的类型，例如作业传输的事件。
        /// </summary>
        /// <param name="Val">标识要接收的事件。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int SetNotifyFlags(BG_JOB_NOTIFICATION_TYPE Val);

        /// <summary>
        /// 检索作业的事件通知标志。
        /// </summary>
        /// <param name="pVal">标识应用程序接收的事件。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetNotifyFlags(out BG_JOB_NOTIFICATION_TYPE pVal);

        /// <summary>
        /// 标识对 BITS 的 IBackgroundCopyCallback 接口的实现。 使用 IBackgroundCopyCallback 接口接收作业相关事件的通知。
        /// </summary>
        /// <param name="Val">IBackgroundCopyCallback 接口指针。 若要删除当前回调接口指针，请将此参数设置为 NULL。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int SetNotifyInterface(IntPtr Val);

        /// <summary>
        /// 检索指向 IBackgroundCopyCallback 接口实现的接口指针。
        /// </summary>
        /// <param name="pVal">指向 IBackgroundCopyCallback 接口实现的接口指针。 完成后，释放 ppNotifyInterface。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetNotifyInterface(out IntPtr pVal);

        /// <summary>
        /// 设置 BITS 在尝试传输文件之前遇到暂时性错误条件后等待的最短时间长度。
        /// </summary>
        /// <param name="Seconds">BITS 在尝试传输文件之前遇到暂时性错误后等待的最短时间长度（以秒为单位）。 默认重试延迟为 600 秒 (10 分钟) 。 可以指定的最小重试延迟为 5 秒。 如果指定的值小于 5 秒，则 BITS 会将该值更改为 5 秒。 如果该值超过从 GetNoProgressTimeout 方法检索到的 no-progress-timeout 值，则 BITS 不会重试传输并将作业移动到BG_JOB_STATE_ERROR状态。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int SetMinimumRetryDelay(uint Seconds);

        /// <summary>
        /// 检索服务在尝试传输文件之前遇到暂时性错误条件后等待的最短时间长度。
        /// </summary>
        /// <param name="Seconds">服务在尝试传输文件之前遇到暂时性错误后等待的时间长度（以秒为单位）。</param>
        /// <returns>此方法在成功时返回 S_OK 或错误时返回标准 COM HRESULT 值之一。</returns>
        [PreserveSig]
        int GetMinimumRetryDelay(out uint Seconds);

        /// <summary>
        /// 设置发生暂时性错误条件后 BITS 尝试传输文件的时间长度。 如果有进度，则重置计时器。
        /// </summary>
        /// <param name="Seconds">出现第一个暂时性错误后 BITS 尝试传输文件的时间长度（以秒为单位）。 默认重试时间为 1,209,600 秒， (14 天) 。 将重试期设置为 0 以防止重试，并强制作业进入所有错误的BG_JOB_STATE_ERROR状态。 如果重试周期值超过 JobInactivityTimeout 组策略值 (90 天默认) ，则 BITS 在超过策略值后取消作业。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int SetNoProgressTimeout(uint Seconds);

        /// <summary>
        /// 检索发生暂时性错误条件后服务尝试传输文件的时间长度。 如果有进度，则重置计时器。
        /// </summary>
        /// <param name="Seconds">发生暂时性错误后，服务尝试传输文件的时间长度（以秒为单位）。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetNoProgressTimeout(out uint Seconds);

        /// <summary>
        /// 检索 BITS 尝试传输作业并发生错误的次数。
        /// </summary>
        /// <param name="Errors">BITS 尝试传输作业时发生的错误数。 当作业从BG_JOB_STATE_TRANSFERRING状态移动到BG_JOB_STATE_TRANSIENT_ERROR或BG_JOB_STATE_ERROR状态时，计数会增加。</param>
        /// <returns>此方法在成功时返回 S_OK 或错误时返回标准 COM HRESULT 值之一。</returns>
        [PreserveSig]
        int GetErrorCount(out uint Errors);

        /// <summary>
        /// 指定用于传输文件的代理。
        /// </summary>
        /// <param name="ProxyUsage">指定是使用用户的代理设置，不使用代理，还是使用应用程序指定的代理设置。 默认使用用户的代理设置， BG_JOB_PROXY_USAGE_PRECONFIG。 有关代理选项的列表，请参阅 BG_JOB_PROXY_USAGE 枚举。</param>
        /// <param name="ProxyList">
        /// 以 Null 结尾的字符串，其中包含用于传输文件的代理。 列表以空格分隔。
        /// 如果 ProxyUsage 的值BG_JOB_PROXY_USAGE_PRECONFIG、BG_JOB_PROXY_USAGE_NO_PROXY或BG_JOB_PROXY_USAGE_AUTODETECT，则此参数必须为 NULL。
        /// 代理列表的长度限制为 4,000 个字符，不包括 null 终止符。
        /// </param>
        /// <param name="ProxyBypassList">
        /// 以 Null 结尾的字符串，其中包含可绕过代理的主机名和/或 IP 地址的可选列表。 列表以空格分隔。
        /// 如果 ProxyUsage 的值BG_JOB_PROXY_USAGE_PRECONFIG、BG_JOB_PROXY_USAGE_NO_PROXY或BG_JOB_PROXY_USAGE_AUTODETECT，则此参数必须为 NULL。
        /// 代理绕过列表的长度限制为 4,000 个字符，不包括 null 终止符。
        /// </param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int SetProxySettings(BG_JOB_PROXY_USAGE ProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string ProxyList, [MarshalAs(UnmanagedType.LPWStr)] string ProxyBypassList);

        /// <summary>
        /// 检索作业用于传输文件的代理信息。
        /// </summary>
        /// <param name="pProxyUsage">指定作业用于传输文件的代理设置。 有关代理选项的列表，请参阅 BG_JOB_PROXY_USAGE 枚举。</param>
        /// <param name="pProxyList">以 Null 结尾的字符串，其中包含用于传输文件的一个或多个代理。 列表以空格分隔。 有关字符串格式的详细信息，请参阅 启用 Internet 功能的列出代理服务器部分。 完成后，调用 CoTaskMemFree 函数以释放 ppProxyList 。</param>
        /// <param name="pProxyBypassList">以 Null 结尾的字符串，其中包含未通过代理路由的主机名或 IP 地址的可选列表或两者。 列表以空格分隔。 有关字符串格式的详细信息，请参阅 启用 Internet 功能的列出代理绕过部分。 完成后，调用 CoTaskMemFree 函数以释放 ppProxyBypassList 。</param>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int GetProxySettings(out BG_JOB_PROXY_USAGE pProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyList, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyBypassList);

        /// <summary>
        /// 将作业的所有权更改为当前用户。
        /// </summary>
        /// <returns>此方法返回以下 HRESULT 值以及其他值。</returns>
        [PreserveSig]
        int TakeOwnership();
    }
}
