using GetStoreApp.Extensions.DataType.Events;

namespace GetStoreApp.Extensions.DataType.Delegates
{
    public delegate void ListItemsChangedEventHandler<T>(object sender, ItemsChangedEventArgs<T> args);
}
