using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用信息数据模型
    /// </summary>
    internal sealed partial class AppInfoModel : INotifyPropertyChanged
    {
        private string _name = string.Empty;

        internal string Name
        {
            get { return _name; }

            set
            {
                if (!string.Equals(_name, value))
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new(nameof(Name)));
                }
            }
        }

        private string _publisher = string.Empty;

        internal string Publisher
        {
            get { return _publisher; }

            set
            {
                if (!string.Equals(_publisher, value))
                {
                    _publisher = value;
                    PropertyChanged?.Invoke(this, new(nameof(Publisher)));
                }
            }
        }

        private string _description = string.Empty;

        internal string Description
        {
            get { return _description; }

            set
            {
                if (!string.Equals(_description, value))
                {
                    _description = value;
                    PropertyChanged?.Invoke(this, new(nameof(Description)));
                }
            }
        }

        private string _categoryID = string.Empty;

        internal string CategoryID
        {
            get { return _categoryID; }

            set
            {
                if (!string.Equals(_categoryID, value))
                {
                    _categoryID = value;
                    PropertyChanged?.Invoke(this, new(nameof(CategoryID)));
                }
            }
        }

        internal string ProductID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
