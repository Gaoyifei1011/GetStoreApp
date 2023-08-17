using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：始终显示背景色设置控件
    /// </summary>
    public sealed partial class AlwaysShowBackdropControl : Grid, INotifyPropertyChanged
    {
        private bool _alwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set
            {
                _alwaysShowBackdropValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AlwaysShowBackdropControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 开关按钮切换时修改相应设置
        /// </summary>
        public void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
