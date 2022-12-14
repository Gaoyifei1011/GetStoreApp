using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Dialogs.CommonDialogs.About
{
    public class StartupArgsModel : ModelBase
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ArgumentName { get; set; }

        /// <summary>
        /// 具体参数
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        /// 参数是否必需要输入
        /// </summary>
        public string IsRequired { get; set; }

        /// <summary>
        /// 参数具体内容
        /// </summary>
        public string ArgumentContent { get; set; }
    }
}
