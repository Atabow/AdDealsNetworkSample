using AdDealsNetworkLib;
using AdDealsNetworkSample;
using System;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdDealsNetworkSample.iOS.PlatformInfo))]

namespace AdDealsNetworkSample.iOS
{
    public class PlatformInfo : IPlatformInfo
    {
        UIDevice device = new UIDevice();

        public string GetDeviceId()
        {
            return string.Empty;
        }

        public string GetMobileOperatorName()
        {
            return string.Empty;
        }

        public string GetMobileConnectionType()
        {
            return string.Empty;
        }
    }
}
