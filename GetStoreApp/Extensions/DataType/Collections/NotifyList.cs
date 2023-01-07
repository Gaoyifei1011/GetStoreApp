using GetStoreApp.Extensions.DataType.Delegates;
using GetStoreApp.Extensions.DataType.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetStoreApp.Extensions.DataType.Collections
{
    /// <summary>
    /// 扩展列表类，实现带通知的列表，当列表添加或删除项目时，进行通知
    /// </summary>
    public class NotifyList<T> : List<T>
    {
        public static NotifyList<T> Empty
        {
            get { return new NotifyList<T>(); }
        }

        public event ListItemsChangedEventHandler<T> ItemsChanged;

        protected void OnItemsChanged(IList<T> removedItems, IList<T> addedItems)
        {
            ListItemsChangedEventHandler<T> temp = ItemsChanged;
            temp?.Invoke(this, new ItemsChangedEventArgs<T>(removedItems, addedItems));
        }

        public new void Add(T item)
        {
            base.Add(item);

            OnItemsChanged(Empty, new List<T> { item });
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);

            OnItemsChanged(Empty, collection.ToList());
        }

        public new void Clear()
        {
            T[] array = new T[Count];
            CopyTo(array);

            base.Clear();

            OnItemsChanged(array.ToList(), Empty);
        }

        public new bool Remove(T item)
        {
            bool ret = base.Remove(item);
            if (ret) OnItemsChanged(new List<T> { item }, Empty);
            return ret;
        }

        public new int RemoveAll(Predicate<T> match)
        {
            IList<T> removedItems = FindAll(match);

            int count = base.RemoveAll(match);
            if (removedItems.Count != count)
            {
                throw new Exception("[NotifyList][RemoveAll][The number of elements found by the predicate does not match the number of elements removed.]");
            }

            OnItemsChanged(removedItems, Empty);
            return count;
        }

        public new void RemoveAt(int index)
        {
            T removedItem = this[index];
            base.RemoveAt(index);
            OnItemsChanged(new List<T> { removedItem }, Empty);
        }

        public new void RemoveRange(int index, int count)
        {
            IEnumerable<T> range = this.Skip(index + 1).Take(count);
            base.RemoveRange(index, count);
            OnItemsChanged(range.ToList(), Empty);
        }
    }
}
