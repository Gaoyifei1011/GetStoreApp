using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct PROCESSENTRY32
    {
        /// <summary>
        /// 结构大小（以字节为单位）。 在调用 Process32First 函数之前，请将此成员设置为 sizeof(PROCESSENTRY32)。 如果不初始化 dwSize， Process32First 将失败。
        /// </summary>
        public int dwSize;

        /// <summary>
        /// 此成员不再使用，并且始终设置为零。
        /// </summary>
        public uint cntUsage;

        /// <summary>
        /// 进程标识符。
        /// </summary>
        public uint th32ProcessID;

        /// <summary>
        /// 此成员不再使用，并且始终设置为零。
        /// </summary>
        public UIntPtr th32DefaultHeapID;

        /// <summary>
        /// 此成员不再使用，并且始终设置为零。
        /// </summary>
        public uint th32ModuleID;

        /// <summary>
        /// 进程启动的执行线程数。
        /// </summary>
        public uint cntThreads;

        /// <summary>
        /// 创建此过程的进程标识符 (其父进程) 。
        /// </summary>
        public uint th32ParentProcessID;

        /// <summary>
        /// 此过程创建的任何线程的基本优先级。
        /// </summary>
        public int pcPriClassBase;

        /// <summary>
        /// 此成员不再使用，并且始终设置为零。
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// 进程的可执行文件的名称。 若要检索可执行文件的完整路径，请调用 Module32First 函数并检查返回的 MODULEENTRY32 结构的 szExePath 成员。
        /// 但是，如果调用进程是 32 位进程，则必须调用 QueryFullProcessImageName 函数，才能检索 64 位进程的可执行文件的完整路径。
        /// </summary>
        public fixed char szExeFile[Kernel32Library.MAX_PATH];
    }
}
