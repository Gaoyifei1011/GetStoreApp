using GetStoreApp.Extensions.Enum;
using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Settings
{
    public class TraceCleanupModel : ModelBase
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isCleanFailed;

        public bool IsCleanFailed
        {
            get { return _isCleanFailed; }

            set
            {
                _isCleanFailed = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(TraceCleanupModel), new PropertyMetadata(string.Empty));

        public CleanArgs InternalName
        {
            get { return (CleanArgs)GetValue(InternalNameProperty); }
            set { SetValue(InternalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalNameProperty =
            DependencyProperty.Register("InternalName", typeof(string), typeof(TraceCleanupModel), new PropertyMetadata(CleanArgs.History));

        public string CleanFailedText
        {
            get { return (string)GetValue(CleanFailedTextProperty); }
            set { SetValue(CleanFailedTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CleanFailedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CleanFailedTextProperty =
            DependencyProperty.Register("CleanFailedText", typeof(string), typeof(TraceCleanupModel), new PropertyMetadata(string.Empty));
    }
}
