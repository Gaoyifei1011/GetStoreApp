using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// ITaskbarList：公开控制任务栏的方法。 它允许你在任务栏上动态添加、删除和激活项。
    /// ITaskbarList2：通过公开方法将窗口标记为全屏显示来扩展 ITaskbarList 接口。
    /// ITaskbarList3：通过公开支持 Windows 7 中添加的统一启动和切换任务栏按钮功能的方法扩展 ITaskbarList2 。 此功能包括基于选项卡式应用程序中的各个选项卡、缩略图工具栏、通知和状态覆盖以及进度指示器的缩略图表示形式和切换目标。
    /// ITaskbarList4：通过提供一种方法来扩展 ITaskbarList3 , 使调用方能够控制选项卡缩略图和速览功能的两个属性值。
    /// </summary>
    [ComImport, Guid("c43dc798-95d1-4bea-9030-bb99e2983a1a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITaskbarList
    {
        // ITaskbarList 接口方法

        /// <summary>
        /// 初始化任务栏列表对象。必须先调用此方法，然后才能调用任何其他 <see cref="ITaskbarList"> 方法。
        /// </summary>
        [PreserveSig]
        void HrInit();

        /// <summary>
        /// 将项添加到任务栏。
        /// </summary>
        /// <param name="hwnd">要添加到任务栏的窗口的句柄。</param>
        [PreserveSig]
        void AddTab(IntPtr hwnd);

        /// <summary>
        /// 从任务栏中删除项。
        /// </summary>
        /// <param name="hwnd">要从任务栏中删除的窗口的句柄。</param>
        [PreserveSig]
        void DeleteTab(IntPtr hwnd);

        /// <summary>
        /// 激活任务栏上的项。 窗口实际上未激活;任务栏上的窗口项仅显示为活动状态。
        /// </summary>
        /// <param name="hwnd">要显示为活动的任务栏上的窗口的句柄。</param>
        [PreserveSig]
        void ActivateTab(IntPtr hwnd);

        /// <summary>
        /// 将任务栏项标记为活动，但不直观地激活它。
        /// </summary>
        /// <param name="hwnd">要标记为活动的窗口的句柄。</param>
        [PreserveSig]
        void SetActiveAlt(IntPtr hwnd);

        // ITaskbarList2 接口方法

        /// <summary>
        /// 将窗口标记为全屏。
        /// </summary>
        /// <param name="hwnd">要标记的窗口的句柄。</param>
        /// <param name="fFullscreen">一个布尔值，该值标记窗口的所需全屏状态。
        [PreserveSig]
        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3 接口方法

        /// <summary>
        /// 显示或更新任务栏按钮中托管的进度栏，以显示完成完整操作的特定百分比。
        /// </summary>
        /// <param name="hwnd">其关联的任务栏按钮用作进度指示器的窗口的句柄。</param>
        /// <param name="ullCompleted">一个应用程序定义值，该值指示在调用方法时已完成的操作的比例。</param>
        /// <param name="ullTotal">一个应用程序定义的值，该值指定操作完成后将具有值 <param name="ullCompleted"> 。</param>
        [PreserveSig]
        void SetProgressValue(IntPtr hwnd, int ullCompleted, int ullTotal);

        /// <summary>
        /// 设置任务栏按钮上显示的进度指示器的类型和状态。
        /// </summary>
        /// <param name="hwnd">显示操作进度的窗口的句柄。 此窗口的关联任务栏按钮将显示进度栏。</param>
        /// <param name="tbpFlags">控制进度按钮当前状态的标志。 仅指定 <see cref="TBPFLAG"> 标志之一；所有状态都是相互排斥的。</param>
        [PreserveSig]
        void SetProgressState(IntPtr hwnd, TBPFLAG tbpFlags);

        /// <summary>
        /// 通知任务栏，已提供新的选项卡或文档缩略图，以便在应用程序的任务栏组浮出控件中显示。
        /// </summary>
        /// <param name="hwndTab">选项卡或文档窗口的句柄。 此值是必需的，不能为 NULL。</param>
        /// <param name="hwndMDI">应用程序的主窗口的句柄。 此值告知任务栏要将新缩略图附加到哪个应用程序的预览组。 此值是必需的，不能为 NULL。</param>
        [PreserveSig]
        void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);

        /// <summary>
        /// 在应用程序中关闭该选项卡或文档时，从应用程序的预览组中删除缩略图。
        /// </summary>
        /// <param name="hwndTab">正在删除其缩略图的选项卡窗口的句柄。 此值与缩略图通过 <see cref="RegisterTab"> 注册为组的一部分的值相同。 此值是必需的，不能为 NULL。</param>
        [PreserveSig]
        void UnregisterTab(IntPtr hwndTab);

        /// <summary>
        /// 将新缩略图插入选项卡式文档界面， (TDI) 或多文档界面 (MDI) 应用程序的组浮出控件或将现有缩略图移动到应用程序组中的新位置。
        /// </summary>
        /// <param name="hwndTab">要放置其缩略图的选项卡窗口的句柄。 此值是必需的，必须已通过 <see cref="RegisterTab"> 注册，并且不能为 NULL。</param>
        /// <param name="hwndInsertBefore">选项卡窗口的句柄，其缩略图 hwndTab 插入到左侧。 必须已通过 <see cref="RegisterTab"> 注册此句柄。 如果此值为 NULL，则新缩略图将添加到列表末尾。</param>
        [PreserveSig]
        void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);

        /// <summary>
        /// 通知任务栏选项卡或文档窗口已成为活动窗口。
        /// </summary>
        /// <param name="hwndTab">活动选项卡窗口的句柄。 此句柄必须已通过 <see cref="RegisterTab"> 进行注册。 如果没有任何选项卡处于活动状态，则此值可以为 NULL 。</param>
        /// <param name="hwndMDI">应用程序的主窗口的句柄。 此值告知任务栏缩略图所属的组。 此值是必需的，不能为 NULL。</param>
        /// <param name="dwReserved">保留;设置为 0。</param>
        [PreserveSig]
        void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, IntPtr dwReserved);

        /// <summary>
        /// 将具有指定按钮集的缩略图工具栏添加到任务栏按钮浮出控件中窗口的缩略图图像。
        /// </summary>
        /// <param name="hwnd">其缩略图表示形式将接收工具栏的窗口的句柄。 此句柄必须属于调用进程。</param>
        /// <param name="cButtons"><param name="pButtons"> 指向的数组中定义的按钮数。 允许的最大按钮数为 7。</param>
        /// <param name="pButtons">指向 <see cref="THUMBBUTTON"> 结构的数组的指针。 每个 <see cref="THUMBBUTTON"> 定义要添加到工具栏中的单个按钮。 以后无法添加或删除按钮，因此必须是完整的定义集。 按钮也不能重新排序，因此它们在数组中的顺序（即它们从左到右显示的顺序）将是其永久顺序。</param>
        [PreserveSig]
        void ThumbBarAddButtons(IntPtr hwnd, IntPtr cButtons, IntPtr pButtons);

        /// <summary>
        /// 根据窗口的当前状态，显示、启用、禁用或隐藏缩略图工具栏中的按钮。 缩略图工具栏是嵌入任务栏按钮浮出控件中窗口缩略图中的工具栏。
        /// </summary>
        /// <param name="hwnd">其缩略图表示形式包含工具栏的窗口的句柄。</param>
        /// <param name="cButtons"><param name="pButtons"> 指向的数组中定义的按钮数。 允许的最大按钮数为 7。 此数组仅包含表示正在更新的现有按钮的结构。</param>
        /// <param name="pButtons">指向 <see cref="THUMBBUTTON"> 结构的数组的指针。 每个 <see cref="THUMBBUTTON"> 定义要添加到工具栏中的单个按钮。 以后无法添加或删除按钮，因此必须是完整的定义集。 按钮也不能重新排序，因此它们在数组中的顺序（即它们从左到右显示的顺序）将是其永久顺序。</param>
        [PreserveSig]
        void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, IntPtr pButtons);

        /// <summary>
        /// 指定一个图像列表，其中包含任务栏按钮浮出控件中窗口缩略图中嵌入工具栏的按钮图像。
        /// </summary>
        /// <param name="hwnd">其缩略图表示形式包含要更新的工具栏的窗口的句柄。 此句柄必须属于调用进程。</param>
        /// <param name="himl">包含工具栏中使用的所有按钮图像的图像列表的句柄。</param>
        [PreserveSig]
        void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);

        /// <summary>
        /// 将覆盖应用于任务栏按钮，以指示应用程序状态或给用户的通知。
        /// </summary>
        /// <param name="hwnd">关联的任务栏按钮接收覆盖的窗口的句柄。 此句柄必须属于与按钮的应用程序关联的调用进程，并且必须是有效的 HWND 或忽略调用。</param>
        /// <param name="hIcon">要用作覆盖的图标的句柄。 这应该是一个小图标，以 96 dpi 分辨率测量 16x16 像素。 如果覆盖图标已应用于任务栏按钮，则会替换现有覆盖。
        /// 此值可以为 NULL。 如何处理 NULL 值取决于任务栏按钮是表示单个窗口还是一组窗口。
        /// 如果任务栏按钮表示单个窗口，则会从显示中删除覆盖图标。
        /// 如果任务栏按钮表示一组窗口，并且以前的覆盖层仍可用， (早于当前覆盖区接收，但尚未由 NULL 值) 释放，则会显示上一个覆盖层代替当前覆盖。
        /// 调用应用程序在不再需要 hIcon 时负责释放它。 这通常可以在调用 <see cref="SetOverlayIcon"> 后完成，因为任务栏创建并使用其自己的图标副本。
        /// </param>
        /// <param name="pszDescription">指向字符串的指针，该字符串提供覆盖所传达信息的替换文字版本，以提供辅助功能。</param>
        [PreserveSig]
        void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

        /// <summary>
        /// 指定或更新当鼠标指针停留在任务栏按钮浮出控件中单个预览缩略图时显示的工具提示的文本。
        /// </summary>
        /// <param name="hwnd">其缩略图显示工具提示的窗口的句柄。 此句柄必须属于调用进程。</param>
        /// <param name="pszTip">指向工具提示中显示的文本的指针。 此值可以为 NULL，在这种情况下， <param name="hwnd"> 指定的窗口标题用作工具提示。</param>
        [PreserveSig]
        void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

        /// <summary>
        /// 选择窗口工作区的一部分，以在任务栏中显示为该窗口的缩略图。
        /// </summary>
        /// <param name="hwnd">任务栏中表示的窗口的句柄。</param>
        /// <param name="prcClip">指向 RECT 结构的指针，该结构指定窗口工作区中相对于该工作区左上角的选择。 若要清除已到位的剪辑并返回到缩略图的默认显示，请将此参数设置为 NULL。</param>
        [PreserveSig]
        void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);

        // ITaskbarList 接口方法

        /// <summary>
        /// 允许选项卡指定在某些情况下，主应用程序框架窗口或选项卡窗口是用作缩略图还是在速览功能中。
        /// </summary>
        /// <param name="hwndTab">要设置属性的选项卡窗口的句柄。此句柄必须已通过注册选项卡注册。</param>
        /// <param name="stpFlags"><see cref="STPFLAG"> 枚举的一个或多个成员，用于指定选项卡缩略图的显示缩略图和速览图像源。</param>
        void SetTabProperties(IntPtr hwndTab, int stpFlags);
    }
}
