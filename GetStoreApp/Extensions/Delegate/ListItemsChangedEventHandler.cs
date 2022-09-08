using GetStoreApp.Extensions.Event;

namespace GetStoreApp.Extensions.Delegate
{
    public delegate void ListItemsChangedEventHandler<T>(object sender, ItemsChangedEventArgs<T> args);
}
