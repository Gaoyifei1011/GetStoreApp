using System.Collections.ObjectModel;
using WinRT;

namespace GetStoreApp.Models
{
    /// <summary>
    /// WinGet 数据源分组模型
    /// </summary>
    [GeneratedBindableCustomProperty]
    public partial class WinGetSourceGroupModel
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public ObservableCollection<WinGetSourceModel> WinGetSourceCollection { get; set; }
    }
}
