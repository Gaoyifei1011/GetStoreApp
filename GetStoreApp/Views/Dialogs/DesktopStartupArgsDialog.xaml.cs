using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 桌面程序参数对话框
    /// </summary>
    internal sealed partial class DesktopStartupArgsDialog : ContentDialog
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string ChannelString = ResourceService.GetLocalized("Dialog/Channel");
        private readonly string LinkContentString = ResourceService.GetLocalized("Dialog/LinkContent");
        private readonly string LinkString = ResourceService.GetLocalized("Dialog/Link");
        private readonly string NoString = ResourceService.GetLocalized("Dialog/No");
        private readonly string TypeString = ResourceService.GetLocalized("Dialog/Type");
        private readonly string YesString = ResourceService.GetLocalized("Dialog/Yes");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private List<StartupArgsModel> DesktopStartupArgsList { get; } = [];

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal DesktopStartupArgsDialog()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            DesktopStartupArgsList.Add(new()
            {
                ArgumentName = TypeString,
                Argument = "-t; --type",
                IsRequired = NoString,
                ArgumentContent = @"""url"",""pid"""
            });
            DesktopStartupArgsList.Add(new()
            {
                ArgumentName = ChannelString,
                Argument = "-c; --channel",
                IsRequired = NoString,
                ArgumentContent = @"""wif"",""wis"",""rp"",""rt"""
            });
            DesktopStartupArgsList.Add(new()
            {
                ArgumentName = LinkString,
                Argument = "-l; --link",
                IsRequired = YesString,
                ArgumentContent = LinkContentString
            });
        }

        #endregion 第四部分：数据操作与业务逻辑
    }
}
