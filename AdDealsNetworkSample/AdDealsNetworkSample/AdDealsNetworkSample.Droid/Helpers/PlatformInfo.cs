using System;
using Android.OS;
using Xamarin.Forms;
using AdDealsNetworkLib;

[assembly: Dependency(typeof(AdDealsNetworkSample.Droid.PlatformInfo))]

namespace AdDealsNetworkSample.Droid
{
    public class PlatformInfo : IPlatformInfo
    {
        public string GetDeviceId()
        {
            return Build.Device;
        }

        public string GetMobileOperatorName()
        {
            // TODO: Get mobile operator name
            return string.Empty;
        }

        public string GetMobileConnectionType()
        {
            return string.Empty;
        }
    }
}
