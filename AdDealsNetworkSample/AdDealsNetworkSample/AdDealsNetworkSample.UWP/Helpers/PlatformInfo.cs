using AdDealsNetworkLib;
using System;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdDealsNetworkSample.UWP.PlatformInfo))]

namespace AdDealsNetworkSample.UWP
{
    public class PlatformInfo : IPlatformInfo
    {
        EasClientDeviceInformation devInfo = new EasClientDeviceInformation();

        public string GetDeviceId()
        {
            return devInfo.Id.ToString();
        }

        public string GetMobileOperatorName()
        {
            return string.Empty;
        }

        public string GetMobileConnectionType()
        {
            var avail = NetworkInterface.GetIsNetworkAvailable();
            if (!avail)
            {
                return "CELLULAR_UNKNOWN";
            }

            var profile = NetworkInformation.GetInternetConnectionProfile();
            var ianaInterfaceType = profile.NetworkAdapter.IanaInterfaceType;

            string appConnection = string.Empty;
            if (ianaInterfaceType <= 71U)
            {
                if ((int)ianaInterfaceType != 6)
                {
                    if ((int)ianaInterfaceType != 71)
                    {
                        return appConnection;
                    }

                    appConnection = "WIFI";
                }
                else
                {
                    appConnection = "ETHERNET";
                }
            }
            else
            {
                switch (ianaInterfaceType)
                {
                    case 216:
                        appConnection = "GPRS";
                        break;
                    case 243:
                        appConnection = "3G";
                        break;
                    case 244:
                        appConnection = "4G";
                        break;
                }
            }

            return appConnection;
        }
    }
}
