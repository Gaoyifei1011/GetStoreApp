using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 定义可用于“打开”或“保存”对话框的选项集
    /// </summary>
    [Flags]
    public enum FILEOPENDIALOGOPTIONS : uint
    {
        /// <summary>
        /// 保存文件时，在覆盖同名的现有文件之前进行提示。这是“保存”对话框的默认值。
        /// </summary>
        FOS_OVERWRITEPROMPT = 0x2,

        /// <summary>
        /// 在“保存”对话框中，仅允许用户选择具有通过 <see cref="IFileOpenDialog.SetFileTypes"> 指定的文件扩展名之一的文件。
        /// </summary>
        FOS_STRICTFILETYPES = 0x4,

        /// <summary>
        /// 不更改当前工作目录。
        /// </summary>
        FOS_NOCHANGEDIR = 0x8,

        /// <summary>
        /// 显示一个“打开”对话框，该对话框提供文件夹而不是文件选项。
        /// </summary>
        FOS_PICKFOLDERS = 0x20,

        /// <summary>
        /// 确保返回的项目是文件系统项目 <see cref="SFGAO.SFGAO_FILESYSTEM">。请注意，这不适用于 <see cref="IFileOpenDialog.GetCurrentSelection"> 返回的项目。
        /// </summary>
        FOS_FORCEFILESYSTEM = 0x40,

        /// <summary>
        /// 使用户能够选择 Shell 命名空间中的任何项，而不仅仅是具有 <see cref="SFGAO.SFGAO_STREAM"> 或 <see cref="SFGAO.SFGAO_FILESYSTEM"> 属性的项。
        /// 此标志不能与 <see cref="FOS_FORCEFILESYSTEM"> 结合使用。
        /// </summary>
        FOS_ALLNONSTORAGEITEMS = 0x80,

        /// <summary>
        /// 不检查会阻止应用程序打开所选文件的情况，例如共享冲突或拒绝访问错误。
        /// </summary>
        FOS_NOVALIDATE = 0x100,

        /// <summary>
        /// 允许用户在打开的对话框中选择多个项目。请注意，设置此标志时，必须使用 <see cref="IFileOpenDialog"> 接口来检索这些项。
        /// </summary>
        FOS_ALLOWMULTISELECT = 0x200,

        /// <summary>
        /// 返回的项目必须位于现有文件夹中。这是默认值。
        /// </summary>
        FOS_PATHMUSTEXIST = 0x800,

        /// <summary>
        /// 返回的项必须存在。这是“打开”对话框的默认值。
        /// </summary>
        FOS_FILEMUSTEXIST = 0x1000,

        /// <summary>
        /// 如果保存对话框中返回的项目不存在，则提示创建。请注意，这实际上不会创建项目。
        /// </summary>
        FOS_CREATEPROMPT = 0x2000,

        /// <summary>
        /// 如果在应用程序打开文件时出现共享冲突，请通过 OnShareViolation 回调应用程序以获取指导。此标志被 <see cref="FOS_NOVALIDATE"> 覆盖。
        /// </summary>
        FOS_SHAREAWARE = 0x4000,

        /// <summary>
        /// 不返回只读项。这是“保存”对话框的默认值。
        /// </summary>
        FOS_NOREADONLYRETURN = 0x8000,

        /// <summary>
        /// 不测试“保存”对话框中指定的项的创建是否会成功。如果未设置此标志，则调用应用程序必须处理创建项时发现的错误，例如拒绝访问。
        /// </summary>
        FOS_NOTESTFILECREATE = 0x10000,

        /// <summary>
        /// 隐藏用户最近打开或保存项目的位置列表。此值在 Windows 7 中不受支持。
        /// </summary>
        FOS_HIDEMRUPLACES = 0x20000,

        /// <summary>
        /// 隐藏视图导航窗格中默认显示的项目。此标志通常与 <see cref="IFileOpenDialog.AddPlace"> 方法结合使用，以隐藏标准位置并将其替换为自定义位置。
        /// Windows 7 及更高版本：隐藏导航窗格中显示的所有标准命名空间位置（如收藏夹、库、计算机和网络）。
        /// Windows Vista：在导航窗格中隐藏“收藏夹链接”树的内容。请注意，类别本身仍会显示，但显示为空。
        /// </summary>
        FOS_HIDEPINNEDPLACES = 0x40000,

        /// <summary>
        /// 不应将快捷方式视为其目标项。这允许应用程序打开.lnk文件，而不是该文件的快捷方式。
        /// </summary>
        FOS_NODEREFERENCELINKS = 0x100000,

        /// <summary>
        /// 在用户导航视图或编辑文件名（如果适用）之前，“确定”按钮将被禁用。注意：禁用“确定”按钮不会阻止通过 Enter 键提交对话框。
        /// </summary>
        FOS_OKBUTTONNEEDSINTERACTION = 0x200000,

        /// <summary>
        /// 不要将正在打开或保存的项目添加到最近的文档列表 （SHAddToRecentDocs）。
        /// </summary>
        FOS_DONTADDTORECENT = 0x2000000,

        /// <summary>
        /// 包括隐藏项和系统项。
        /// </summary>
        FOS_FORCESHOWHIDDEN = 0x10000000,

        /// <summary>
        /// 指示“另存为”对话框应以展开模式打开。扩展模式是通过单击“另存为”对话框左下角的按钮来设置和取消设置的模式，该按钮在单击时在“浏览文件夹”和“隐藏文件夹”之间切换。此值在 Windows 7 中不受支持。
        /// </summary>
        FOS_DEFAULTNOMINIMODE = 0x20000000,

        /// <summary>
        /// 指示“打开”对话框应始终显示预览窗格。
        /// </summary>
        FOS_FORCEPREVIEWPANEON = 0x40000000,

        /// <summary>
        /// 指示调用方正在以流 （BHID_Stream） 的形式打开文件，因此无需下载该文件。
        /// </summary>
        FOS_SUPPORTSTREAMABLEITEMS = 0x80000000
    };
}
