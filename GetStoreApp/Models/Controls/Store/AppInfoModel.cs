using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.Models.Controls.Store
{
    /// <summary>
    /// 应用信息数据模型
    /// </summary>
    public class AppInfoModel : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }

            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _publisher;

        public string Publisher
        {
            get { return _publisher; }

            set
            {
                _publisher = value;
                OnPropertyChanged();
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }

            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _categoryID;

        public string CategoryID
        {
            get { return _categoryID; }

            set
            {
                _categoryID = value;
                OnPropertyChanged();
            }
        }

        public string ProductID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
