namespace GetStoreApp.WindowsAPI.Controls.Taskbar
{
    /// <summary>
    /// 由 <see cref="ITaskbarList4.SetTabProperties"> 方法用来指定选项卡属性
    /// </summary>
    internal enum STPFLAG
    {
        /// <summary>
        /// 未指定任何特定属性值。 使用默认行为：选项卡窗口提供缩略图和速览图像，根据需要实时或静态。
        /// </summary>
        STPF_NONE = 0x0,

        /// <summary>
        /// 始终使用主应用程序框架窗口提供的缩略图，而不是单个选项卡窗口提供的缩略图。 请勿将此值与 <see cref="STPF_USEAPPTHUMBNAILWHENACTIVE"> 合并;这样做将导致错误。
        /// </summary>
        STPF_USEAPPTHUMBNAILALWAYS = 0x1,

        /// <summary>
        /// 当应用程序选项卡处于活动状态并且其窗口的实时表示形式可用时，请使用主应用程序的框架窗口缩略图。
        /// 在其他情况下，请使用选项卡窗口缩略图。 请勿将此值与 <see cref="STPF_USEAPPTHUMBNAILALWAYS"> 合并;这样做将导致错误。
        /// </summary>
        STPF_USEAPPTHUMBNAILWHENACTIVE = 0x2,

        /// <summary>
        /// 始终使用主应用程序框架窗口提供的速览图像，而不是单个选项卡窗口提供的速览图像。 请勿将此值与 <see cref="STPF_USEAPPTHUMBNAILWHENACTIVE"> 合并;这样做将导致错误。
        /// </summary>
        STPF_USEAPPPEEKALWAYS = 0x4,

        /// <summary>
        /// 当应用程序选项卡处于活动状态并且其窗口的实时表示形式可用时，在速览功能中显示主应用程序框架。 在其他情况下，请使用选项卡窗口。 请勿将此值与 <see cref="STPF_USEAPPTHUMBNAILALWAYS"> 合并;这样做将导致错误。
        /// </summary>
        STPF_USEAPPPEEKWHENACTIVE = 0x8
    }
}
