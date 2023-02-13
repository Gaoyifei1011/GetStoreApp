namespace GetStoreApp.WindowsAPI.Controls.Taskbar
{
    /// <summary>
    /// 提供对 <see cref="ITaskbarList4"> 接口提供的功能的内部访问，而无需强制通过另一个单例引用它。
    /// </summary>
    internal static class TaskbarList
    {
        private static object _syncLock = new object();

        private static ITaskbarList4 _taskbarList;

        internal static ITaskbarList4 Instance
        {
            get
            {
                if (_taskbarList is null)
                {
                    lock (_syncLock)
                    {
                        if (_taskbarList is null)
                        {
                            _taskbarList = (ITaskbarList4)new CTaskbarList();
                            _taskbarList.HrInit();
                        }
                    }
                }

                return _taskbarList;
            }
        }
    }
}
