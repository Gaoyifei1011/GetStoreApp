using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Internal.DataTransfer.NearShare;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Helpers.NearShare
{
    public static class NearShareHelper
    {
        private static readonly Guid CLSID_NearShareBroker = new("96274226-3195-4CDE-B0A0-0F6256C7A65A");

        public static ShareSenderBroker CreateShareSenderBroker()
        {
            try
            {
                if (Ole32Library.CoCreateInstance(CLSID_NearShareBroker, 0, CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_ENABLE_CLOAKING, typeof(ICDPComNearShareBroker).GUID, out nint obj) is 0)
                {
                    ICDPComNearShareBroker cdpComNearShareBroker = (ICDPComNearShareBroker)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(obj, CreateObjectFlags.None);
                    if (cdpComNearShareBroker.CreateNearShareSender(out ICDPComNearShareSender cdpComNearShareSender) is 0)
                    {
                        nint shareSenderBrokerObj = Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(cdpComNearShareSender, CreateComInterfaceFlags.None);
                        return ShareSenderBroker.FromAbi(shareSenderBrokerObj);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(NearShareHelper), nameof(CreateShareSenderBroker), 1, e);
                return null;
            }
        }
    }
}
