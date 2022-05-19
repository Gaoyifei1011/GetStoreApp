using GalaSoft.MvvmLight.Messaging;
using GetStoreApp.Core.Models;
using GetStoreApp.Services.Settings;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using System.Collections.Generic;

using Windows.ApplicationModel.Resources.Core;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class HistoryItemValueViewModel : ObservableObject
    {
        // 加载本地资源
        private readonly ResourceContext resourceContext;

        private readonly ResourceMap resourceMap;

        // 主页“历史记录”展示条目设置
        private int _selectedHisItemValue;

        public int SelectedHisItemValue
        {
            get
            {
                _selectedHisItemValue = SimpleHistoryItemSelectorService.SimpleHisItemValue;
                return _selectedHisItemValue;
            }

            set
            {
                SimpleHistoryItemSelectorService.SetSimpleHisItem(value);
                Messenger.Default.Send(value, "SimpleHisItemValue");
                SetProperty(ref _selectedHisItemValue, value);
            }
        }

        // 列表数据
        public List<HistoryItemValueModel> SimpleHisItemList;

        public HistoryItemValueViewModel()
        {
            // UI字符串本地化
            resourceContext = new ResourceContext();

            resourceContext.QualifierValues["Language"] = LanguageSelectorService.PriLangCodeName;

            resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

            // 初始化HisItem列表
            string Set_HisItem_Min = resourceMap.GetValue("Settings_HistoryItem_Minimal", resourceContext).ValueAsString;

            string Set_HisItem_Max = resourceMap.GetValue("Settings_HistoryItem_Maximum", resourceContext).ValueAsString;

            SimpleHisItemList = new List<HistoryItemValueModel>()
            {
                new HistoryItemValueModel(HisItemDisplayName:Set_HisItem_Min,SimpleHisItemValue:3),
                new HistoryItemValueModel(HisItemDisplayName:Set_HisItem_Max,SimpleHisItemValue:5)
            };
        }
    }
}
