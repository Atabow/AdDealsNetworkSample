namespace AdDealsNetworkLib
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public static class AdManager
    {
        const string AppUriFormat = "http://adapi.addealsnetwork.com/addeals/rest/v2/campaigns/openapi?format=json&aid={0}&akey={1}&ctypeid={2}&advuid={3}&lang={4}&country={5}&usragent={6}&adsize={7}&testing={8}&duid={9}&conn={10}&mop={11}";
        const string AppInitUriFormat = "http://adapi.addealsnetwork.com/addeals/tracking/add?advuid={0}&aid={1}&akey={2}&usragent={3}&oapiver=2";

        public static event EventHandler InitSDKSuccess;
        public static event EventHandler InitSDKFailed;

        static bool sdkInitialized = false;
        static string advertisingId = string.Empty;

        // TODO: Get separate app ids for each platform.
        static string country = RegionInfo.CurrentRegion.TwoLetterISORegionName;
        static string lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        static string userAgent = string.Empty;
        static string mop = string.Empty; // mobile operator
        static string mconnection = string.Empty; // mobile connection type
        static string duid = string.Empty;
        static string aid = string.Empty;
        static string appkey = string.Empty;

        static IAdvertisingIdHelper advertisingIdHelper = null;
        static IPlatformInfo platformInfo = null;
        static HybridWebView hybridWebView = null;

        public static void InitSDK(Grid layoutRoot, string appId, string appKey)
        {
            advertisingIdHelper = DependencyService.Get<IAdvertisingIdHelper>();
            platformInfo = DependencyService.Get<IPlatformInfo>();
            hybridWebView = new HybridWebView();

            advertisingIdHelper.GetAdvertisingId(AdManager.SetAdvertisingId);
            duid = WebUtility.UrlEncode(platformInfo.GetDeviceId());
            mconnection = WebUtility.UrlEncode(platformInfo.GetMobileConnectionType());
            mop = WebUtility.UrlEncode(platformInfo.GetMobileOperatorName());

            aid = appId;
            appkey = appKey;

            if (!layoutRoot.Children.Contains(hybridWebView))
            {
                hybridWebView.Uri = "useragent.html";
                hybridWebView.MethodToInvoke = "getUserAgent";
                hybridWebView.WidthRequest = 1;
                hybridWebView.HeightRequest = 1;
                hybridWebView.VerticalOptions = LayoutOptions.Start;
                hybridWebView.HorizontalOptions = LayoutOptions.Start;
                hybridWebView.BackgroundColor = Color.Transparent;
                hybridWebView.Opacity = 1;
                layoutRoot.Children.Add(hybridWebView);
            }

            hybridWebView.RegisterAction(AdManager.SetUserAgent);
        }

        public static async Task<AdDealsPopupAd> GetPopupAd(Grid root, AdKind adDealsAdSize)
        {
            if (sdkInitialized)
            {
                string adSize = string.Empty;
                if (Device.OS == TargetPlatform.Windows)
                {
                    if (IsOrientationLandscape(root))
                    {
                        // Orientation is landscape
                        if (root.Width >= 800 && root.Height >= 480)
                        {
                            adSize = "1024x768";
                        }
                        else
                        {
                            adSize = "800x480";
                        }
                    }
                    else
                    {
                        // Orientation is potrait
                        if (root.Width >= 480 && root.Height >= 800)
                        {
                            adSize = "768x1024";
                        }
                        else
                        {
                            adSize = "480x800";
                        }
                    }
                }
                else if (Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
                {
                    if (IsOrientationLandscape(root))
                    {
                        // Orientation is landscape
                        if (root.Width >= 480 && root.Height >= 320)
                        {
                            adSize = "1024x768";
                        }
                        else
                        {
                            adSize = "480x320";
                        }
                    }
                    else
                    {
                        // Orientation is potrait
                        if (root.Width >= 320 && root.Height >= 480)
                        {
                            adSize = "768x1024";
                        }
                        else
                        {
                            adSize = "320x480";
                        }
                    }
                }

                string appUri = string.Format(
                        AppUriFormat, aid, appkey, 3, advertisingId, lang, country, userAgent, adSize, 0, duid, mconnection, mop);
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string content = await client.GetStringAsync(new Uri(appUri));
                        if (content != "[]")
                        {
                            AdDealsContent[] adDealsContent = JsonConvert.DeserializeObject<AdDealsContent[]>(content);
                            if (adDealsContent.Length > 0)
                            {
                                // Ad fetched successfully.
                                return new AdDealsPopupAd(root, adDealsContent, true);

                            }
                        }
                        else
                        {
                            // No ads available.
                            return new AdDealsPopupAd(root, new AdDealsContent[0], false);
                        }
                    }
                }
                catch (Exception)
                {
                    return new AdDealsPopupAd(root, new AdDealsContent[0], false);
                }
            }

            // SDK not initialized correctly.
            return new AdDealsPopupAd(root, new AdDealsContent[0], false);
        }

        static async Task Initialize()
        {
            if (!sdkInitialized && !string.IsNullOrEmpty(advertisingId) && !string.IsNullOrEmpty(userAgent))
            {
                string appInitUri = string.Format(AppInitUriFormat, advertisingId, aid, appkey, userAgent);
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(new Uri(appInitUri));
                        response.EnsureSuccessStatusCode();
                        sdkInitialized = true;

                        InitSDKSuccess?.Invoke(new object(), new EventArgs());
                    }
                }
                catch (Exception)
                {
                    sdkInitialized = false;
                    InitSDKFailed?.Invoke(new object(), new EventArgs());
                }
            }
        }

        

        public static async void SetAdvertisingId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                InitSDKFailed?.Invoke(new object(), new EventArgs());
            }
            else
            {
                advertisingId = WebUtility.UrlEncode(id);
                await Initialize();
            }
        }

        public static async void SetUserAgent(string agent)
        {
            if (string.IsNullOrWhiteSpace(agent))
            {
                InitSDKFailed?.Invoke(new object(), new EventArgs());
            }
            else
            {
                userAgent = WebUtility.UrlEncode(agent);
                await Initialize();
            }
        }

        public static void SetMobileOperator(string id)
        {
            mop = WebUtility.UrlEncode(id);
        }

        private static bool IsOrientationLandscape(Grid root)
        {
            return root.Width > root.Height;
        }
    }

    public enum AdKind
    {
        FULLSCREENPOPUPAD = 1,
    }

    public class AdDealsContent
    {
        public int adimageheight;
        public int adimagewidth;
        public string adimageurl;
        public string adtrackinglink;
    }
}
