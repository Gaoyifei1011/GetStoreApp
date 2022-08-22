using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class TraceCleanupViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand TraceCleanupCommand = new AsyncRelayCommand(async () =>
        {
            await new TraceCleanupPromptDialog().ShowAsync();
        });
    }
}
