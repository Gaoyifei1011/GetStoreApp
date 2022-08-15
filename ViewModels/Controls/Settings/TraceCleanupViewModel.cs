using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Helpers;
using GetStoreApp.UI.Dialogs;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class TraceCleanupViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand TraceCleanupCommand { get; }

        public TraceCleanupViewModel()
        {
            TraceCleanupCommand = new AsyncRelayCommand(async () =>
            {
                await new TraceCleanupPromptDialog().ShowAsync();
            });
        }
    }
}
