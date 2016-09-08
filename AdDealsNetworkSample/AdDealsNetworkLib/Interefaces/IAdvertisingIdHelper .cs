namespace AdDealsNetworkLib
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAdvertisingIdHelper
    {
        void GetAdvertisingId(Action<string> callback);
    }
}
