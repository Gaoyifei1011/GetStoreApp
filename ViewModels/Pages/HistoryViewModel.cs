using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Models;
using System.Collections.ObjectModel;
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

        public ICommand EditCommand { get; set; }

        public ICommand CopyCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand CancelCommand { get; set; }

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
