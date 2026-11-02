using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用信息数据模型
    /// </summary>
    internal sealed partial class AppInfoModel : INotifyPropertyChanged
    {
        internal string Name
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(Name)));
                }
            }
        }

        internal string Publisher
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(Publisher)));
                }
            }
        }

        internal string Description
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(Description)));
                }
            }
        }

        internal string CategoryID
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(CategoryID)));
                }
            }
        }

        internal string ProductID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
