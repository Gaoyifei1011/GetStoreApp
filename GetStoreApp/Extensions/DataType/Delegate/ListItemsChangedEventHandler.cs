using GetStoreApp.Extensions.DataType.Event;

namespace GetStoreApp.Extensions.DataType.Delegate
{
    public delegate void ListItemsChangedEventHandler<T>(object sender, ItemsChangedEventArgs<T> args);
}
