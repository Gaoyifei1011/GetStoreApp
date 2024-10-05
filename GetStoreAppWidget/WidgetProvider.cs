using Microsoft.Windows.Widgets.Providers;
using System;

namespace GetStoreAppWidget
{
    /// <summary>
    /// Windows 小组件提供程序实现类
    /// </summary>
    public partial class WidgetProvider : IWidgetProvider
    {
        /// <summary>
        /// 通知小组件提供程序，小组件主机当前有兴趣从提供程序接收更新的内容。
        /// </summary>
        public void Activate(WidgetContext widgetContext)
        {
        }

        /// <summary>
        /// 通知小组件提供程序已创建新小组件，例如，当用户将小组件添加到小组件主机时。
        /// </summary>
        public void CreateWidget(WidgetContext widgetContext)
        {
            string id = widgetContext.Id;
            string definitionId = widgetContext.DefinitionId;
        }

        /// <summary>
        /// 通知小组件提供程序，小组件主机不再主动从提供程序请求更新的内容。
        /// </summary>
        public void Deactivate(string widgetId)
        {
        }

        /// <summary>
        /// 通知小组件提供程序它支持的小组件之一已被删除，例如，当用户从小组件主机中删除小组件时。
        /// </summary>
        public void DeleteWidget(string widgetId, string customState)
        {
            return;
        }

        /// <summary>
        /// 在对小组件（例如用户单击按钮）调用操作时调用。
        /// </summary>
        public void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs)
        {
            string verb = actionInvokedArgs.Verb;

            if (verb is "")
            {
            }
        }

        /// <summary>
        /// 当小组件主机中小组件的配置发生更改时调用。
        /// </summary>
        public void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs)
        {
            WidgetContext widgetContext = contextChangedArgs.WidgetContext;
        }
    }
}
