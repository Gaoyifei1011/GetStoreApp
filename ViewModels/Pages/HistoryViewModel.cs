using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Pages
{
    public class HistoryViewModel : ObservableRecipient
    {
        private bool _isEditMode;

        public bool IsEditMode
        {
            get { return _isEditMode; }

            set { SetProperty(ref _isEditMode, value); }
        }

        private ICommand _editCommand;

        public ICommand EditCommand
        {
            get { return _editCommand; }

            set { SetProperty(ref _editCommand, value); }
        }

        private ICommand _copyCommand;

        public ICommand CopyCommand
        {
            get { return _copyCommand; }

            set { SetProperty(ref _copyCommand, value); }
        }

        private ICommand _deleteCommand;

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }

            set { SetProperty(ref _deleteCommand, value); }
        }

        private ICommand _cancelCommand;

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }

            set { SetProperty(ref _cancelCommand, value); }
        }

        public ObservableCollection<HistoryModel> HistoryList = new ObservableCollection<HistoryModel>();

        public HistoryViewModel()
        {
            EditCommand = new RelayCommand(Edit);

            DeleteCommand = new RelayCommand(Delete);

            CancelCommand = new RelayCommand(Cancel);
        }

        private void Edit()
        {
            IsEditMode = true;
        }

        private void Delete()
        {
            IsEditMode = false;
        }

        private void Cancel()
        {
            IsEditMode = false;
        }
    }
}
