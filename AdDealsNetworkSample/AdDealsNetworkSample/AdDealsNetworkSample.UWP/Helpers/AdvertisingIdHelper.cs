using AdDealsNetworkLib;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdDealsNetworkSample.UWP.AdvertisingIdHelper))]

namespace AdDealsNetworkSample.UWP
{
    class AdvertisingIdHelper : IAdvertisingIdHelper
    {
        public void GetAdvertisingId(Action<string> callback)
        {
            callback(Windows.System.UserProfile.AdvertisingManager.AdvertisingId);
        }
    }
}
