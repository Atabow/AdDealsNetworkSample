using System.Threading.Tasks;
using Xamarin.Forms;
using UIKit;
using System;
using AdSupport;
using AdDealsNetworkLib;

[assembly: Dependency(typeof(AdDealsNetworkSample.iOS.AdvertisingIdHelper))]
namespace AdDealsNetworkSample.iOS
{
    class AdvertisingIdHelper : IAdvertisingIdHelper
    {
        public void GetAdvertisingId(Action<string> callback)
        {
            callback(ASIdentifierManager.SharedManager.AdvertisingIdentifier.AsString());
        }
    }
}
