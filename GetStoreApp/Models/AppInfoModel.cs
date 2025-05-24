using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用信息数据模型
    /// </summary>
    public sealed partial class AppInfoModel : INotifyPropertyChanged
    {
        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }

            set
            {
                if (!Equals(_name, value))
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        private string _publisher = string.Empty;

        public string Publisher
        {
            get { return _publisher; }

            set
            {
                if (!Equals(_publisher, value))
                {
                    _publisher = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Publisher)));
                }
            }
        }

        private string _description = string.Empty;

        public string Description
        {
            get { return _description; }

            set
            {
                if (!Equals(_description, value))
                {
                    _description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
                }
            }
        }

        private string _categoryID = string.Empty;

        public string CategoryID
        {
            get { return _categoryID; }

            set
            {
                if (!Equals(_categoryID, value))
                {
                    _categoryID = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoryID)));
                }
            }
        }

        public string ProductID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
