using AdDealsNetworkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AdDealsNetworkSample
{
    public partial class MainPage : ContentPage
    {
        const string DROIDAPPID = "<APPID>";
        const string DROIDAPPKEY = "<APPKEY>";

        const string IOSAPPID = "";
        const string IOSAPPKEY = "";

        const string WINAPPID = "<APPID>";
        const string WINAPPKEY = "<APPKEY>";

        public MainPage()
        {
            InitializeComponent();

            string appId = Device.OnPlatform(IOSAPPID, DROIDAPPID, WINAPPID);
            string appKey = Device.OnPlatform(IOSAPPKEY, DROIDAPPKEY, WINAPPKEY);
            AdManager.InitSDK(this.MainGrid, appId, appKey);
        }

        async void OnShowAdDealsFullPageAdClicked(object sender, EventArgs args)
        {
            AdDealsPopupAd popupAdToShow = await AdManager.GetPopupAd(this.MainGrid, AdKind.FULLSCREENPOPUPAD);
            popupAdToShow.ShowAd();
        }
    }
}
