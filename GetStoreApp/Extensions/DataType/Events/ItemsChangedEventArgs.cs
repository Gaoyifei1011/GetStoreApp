using System;
using System.Collections.Generic;

namespace GetStoreApp.Extensions.DataType.Events
{
    /// <summary>
    /// 扩展列表类，实现带通知的列表，当列表添加或删除项目时，进行通知
    /// </summary>
    public class ItemsChangedEventArgs<T> : EventArgs
    {
        public IList<T> RemovedItems { get; private set; }
        public IList<T> AddedItems { get; private set; }

        public ItemsChangedEventArgs(IList<T> removedItems, IList<T> addItems)
        {
            RemovedItems = removedItems;
            AddedItems = addItems;
        }
    }
}
