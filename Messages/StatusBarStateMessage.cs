using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// StatusBar状态栏状态消息
    /// </summary>
    public class StatusBarStateMessage : ValueChangedMessage<int>
    {
        public StatusBarStateMessage(int value):base(value)
        {
        }
    }
}
