namespace AdDealsNetworkLib
{
    using System;
    using System.Diagnostics;
    using Xamarin.Forms;

    public class AdDealsPopupAd : ContentView
    {
        const string NoAd = "http://NoAd.png"; // Note this URI does not exists 
        static Stopwatch showAdFreq = new Stopwatch();
        string adTrackingLink = string.Empty;
        string adImageUrl = string.Empty;

        double aspectRatio = 1.0;
        Grid PopupGrid;
        Grid CloseButtonGrid;
        Label CloseButton;
        Image AdDealsImage;
        Grid root;
        bool isSDKInitialized = false;
        TimeSpan minDelayBtwAdRequest = TimeSpan.FromSeconds(3);

        public delegate void AdDealsEventHandler(object sender, EventArgs e);

        public event EventHandler AdClosed; // OPTIONAL. This is triggered when the popup ad is closed. 
        public event EventHandler AdClicked; //OPTIONAL. This is triggered when an ad is clicked by end user. 
        public event EventHandler ShowAdFailed; // OPTIONAL. This is triggered when no ad is available or an issue occurs (slow network connection...) 
        public event EventHandler ShowAdSuccess; // OPTIONAL. This is triggered when an ad is displayed to end user. 
        public event EventHandler MinDelayBtwAdsNotReached; // OPTIONAL. This is triggered when you try to call more than 1 ad in a very short period of time (less than 3 sec). 
        public event EventHandler SDKNotInitialized; // OPTIONAL. This is triggered when you try to load an ad without initializing the SDK. 

        public AdDealsPopupAd(Grid layoutRoot, AdDealsContent[] adDealsContent, bool sdkInitialized)
        {
            this.root = layoutRoot;
            this.isSDKInitialized = sdkInitialized;
            this.adTrackingLink = adDealsContent.Length > 0 ? adDealsContent[0].adtrackinglink : NoAd;
            this.adImageUrl = adDealsContent.Length > 0 ? adDealsContent[0].adimageurl : NoAd;
            int adImageHeight = adDealsContent.Length > 0 ? adDealsContent[0].adimageheight : 320;
            int adImageWidth = adDealsContent.Length > 0 ? adDealsContent[0].adimagewidth : 320;

            this.CloseButtonGrid = new Grid
            {
                WidthRequest = 50,
                HeightRequest = 50,
                Padding = 5,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
            };

            this.CloseButton = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 38,
                TextColor = Color.White,
                BackgroundColor = Color.Black,
                FontAttributes = FontAttributes.Bold,
                Text = "X",
            };

            CloseButtonGrid.Children.Add(this.CloseButton);

            TapGestureRecognizer tapGesture1 = new TapGestureRecognizer();
            tapGesture1.NumberOfTapsRequired = 1;
            tapGesture1.Tapped += OnCloseImageClicked;
            this.CloseButton.GestureRecognizers.Add(tapGesture1);

            this.AdDealsImage = new Image
            {
                Source = new UriImageSource
                {
                    Uri = new Uri(this.adImageUrl)
                },
                WidthRequest = adImageWidth,
                HeightRequest = adImageHeight,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            TapGestureRecognizer tapGesture2 = new TapGestureRecognizer();
            tapGesture2.NumberOfTapsRequired = 1;
            tapGesture2.Tapped += OnAdDealsImageTapped;
            this.AdDealsImage.GestureRecognizers.Add(tapGesture2);

            this.PopupGrid = new Grid
            {
                Children =
                {
                    this.AdDealsImage,
                    this.CloseButtonGrid,
                },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            var grid = new Grid
            {
                Children =
                {
                    this.PopupGrid
                },
                Padding = 5,
                BackgroundColor = Color.FromHex("#FF888888"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            this.Content = grid;
            this.Content.VerticalOptions = LayoutOptions.Center;
            this.Content.HorizontalOptions = LayoutOptions.Center;

            this.BackgroundColor = Color.FromHex("#C0000000");
            this.aspectRatio = adImageWidth > adImageHeight ? (double)adImageWidth / adImageHeight : (double)adImageHeight / adImageWidth;
            this.PopupGrid.SizeChanged += AdDealsPopupAd_SizeChanged;
        }

        private void AdDealsPopupAd_SizeChanged(object sender, EventArgs e)
        {
            var grid = (sender as Grid);

            if (this.Width > this.Height)
            {
                grid.WidthRequest = grid.Bounds.Height * this.aspectRatio;
            }
            else
            {
                grid.HeightRequest = grid.Bounds.Width * this.aspectRatio;
            }
        }

        public void ShowAd()
        {
            if (!this.isSDKInitialized)
            {
                this.SDKNotInitialized?.Invoke(new object(), new EventArgs());
                this.RemoveViewFromPage();
            }
            else if (this.adTrackingLink.Equals(NoAd, StringComparison.OrdinalIgnoreCase) ||
                this.adImageUrl.Equals(NoAd, StringComparison.OrdinalIgnoreCase))
            {
                this.ShowAdFailed?.Invoke(new object(), new EventArgs());
                this.RemoveViewFromPage();
            }
            else
            {
                if (!showAdFreq.IsRunning || showAdFreq.Elapsed > this.minDelayBtwAdRequest)
                {
                    if (!showAdFreq.IsRunning)
                    {
                        showAdFreq.Start();
                    }
                    else
                    {
                        showAdFreq.Restart();
                    }

                    this.ShowAdSuccess?.Invoke(new object(), new EventArgs());
                    this.AddViewToPage();
                }
                else
                {
                    this.MinDelayBtwAdsNotReached?.Invoke(new object(), new EventArgs());
                    this.RemoveViewFromPage();
                }
            }
        }

        void OnAdDealsImageTapped(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(this.adTrackingLink))
            {
                Device.OpenUri(new Uri(this.adTrackingLink));
                this.AdClicked?.Invoke(new object(), new EventArgs());
            }
        }

        void AddViewToPage()
        {
            if (!this.root.Children.Contains(this))
            {
                this.root.Children.Add(this);
            }

            this.IsVisible = true;
        }

        void RemoveViewFromPage()
        {
            if (this.root.Children.Contains(this))
            {
                this.root.Children.Remove(this);
            }

            this.IsVisible = false;
        }

        void OnCloseImageClicked(object sender, EventArgs args)
        {
            this.AdClosed?.Invoke(new object(), new EventArgs());
            this.RemoveViewFromPage();
        }
    }
}
