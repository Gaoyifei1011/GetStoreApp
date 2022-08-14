using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class TraceCleanupPromptViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand TraceCleanupSureCommand { get; }

        public IAsyncRelayCommand TraceCleanupCancelCommand { get; }

        public TraceCleanupPromptViewModel()
        {
            //FileCleanSureCommand = new AsyncRelayCommand();

            TraceCleanupCancelCommand = new AsyncRelayCommand<ContentDialog>(async (param) =>
            {
                param.Hide();
                await Task.CompletedTask;
            });
        }
    }
}
