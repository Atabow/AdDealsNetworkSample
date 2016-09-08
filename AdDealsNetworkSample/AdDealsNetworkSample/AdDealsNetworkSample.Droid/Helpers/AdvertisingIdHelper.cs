using Android.App;
using Android.OS;
using Android.Views;
using System.Threading.Tasks;
using Xamarin.Forms;
using System;
using Android.Gms.Common;
using Android.Gms.Ads.Identifier;
using Android.Util;
using Android.Content;
using Android.Widget;
using AdDealsNetworkLib;

[assembly: Dependency(typeof(AdDealsNetworkSample.Droid.AdvertisingIdHelper))]

namespace AdDealsNetworkSample.Droid
{
    public class AdvertisingIdHelper : IAdvertisingIdHelper
    {
        public void GetAdvertisingId(Action<string> callback)
        {
            string advertisingId = string.Empty;
            Activity activity = Xamarin.Forms.Forms.Context as Activity;
            Task task = Task.Factory.StartNew(() =>
            {
                try
                {
                    AdvertisingIdClient.Info adinfo = AdvertisingIdClient.GetAdvertisingIdInfo(activity);
                    callback(adinfo.Id);
                }
                catch (Exception)
                {
                    callback(string.Empty);
                }
            });
        }
    }
}
