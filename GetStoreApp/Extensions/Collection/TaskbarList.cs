using GetStoreApp.Contracts.Extensions;

namespace GetStoreApp.Extensions.Collection
{
    /// <summary>
    /// 提供对 ITaskbarList4 接口提供的功能的内部访问，而无需强制通过另一个单例引用它。
    /// </summary>
    public static class TaskbarList
    {
        private static object _syncLock = new object();

        private static ITaskbarList4 _taskbarList;

        public static ITaskbarList4 Instance
        {
            get
            {
                if (_taskbarList == null)
                {
                    lock (_syncLock)
                    {
                        if (_taskbarList == null)
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
