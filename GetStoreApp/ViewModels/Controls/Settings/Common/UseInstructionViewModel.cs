using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public sealed class UseInstructionViewModel : ViewModelBase
    {
        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set
            {
                _useInsVisValue = value;
                OnPropertyChanged();
            }
        }

        // “使用说明”按钮显示设置
        public IRelayCommand UseInstructionCommand => new RelayCommand<bool>(async (useInsVisValue) =>
        {
            await UseInstructionService.SetUseInsVisValueAsync(useInsVisValue);
            UseInsVisValue = useInsVisValue;
        });

        public UseInstructionViewModel()
        {
            UseInsVisValue = UseInstructionService.UseInsVisValue;
        }
    }
}
