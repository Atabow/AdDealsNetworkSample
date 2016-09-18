namespace AdDealsNetworkLib
{
    using System;
    using System.Diagnostics;
    using Xamarin.Forms;

    public class AdDealsPopupAd : ContentView
    {
        const double CloseButtonWidth = 46;
        const double CloseButtonHeight = 46;
        const string NoAd = "http://NoAd.png"; // Note this URI does not exists 
        static Stopwatch showAdFreq = new Stopwatch();
        string adTrackingLink = string.Empty;
        string adImageUrl = string.Empty;

        Grid PopupGrid;
        Frame CloseButtonFrame;
        Label CloseButton;
        Frame AdDealsImageFrame;
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

            this.CloseButtonFrame = new Frame
            {
                Padding = 3,
                WidthRequest = CloseButtonWidth,
                HeightRequest = CloseButtonHeight,
                OutlineColor = Color.White,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
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

            this.CloseButtonFrame.Content = this.CloseButton;

            TapGestureRecognizer tapGesture1 = new TapGestureRecognizer();
            tapGesture1.NumberOfTapsRequired = 1;
            tapGesture1.Tapped += OnCloseImageClicked;
            this.CloseButton.GestureRecognizers.Add(tapGesture1);

            this.AdDealsImageFrame = new Frame
            {
                Padding = 3,
                OutlineColor = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            this.AdDealsImage = new Image
            {
                Source = new UriImageSource
                {
                    Uri = new Uri(this.adImageUrl)
                },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            this.AdDealsImageFrame.Content = this.AdDealsImage;

            TapGestureRecognizer tapGesture2 = new TapGestureRecognizer();
            tapGesture2.NumberOfTapsRequired = 1;
            tapGesture2.Tapped += OnAdDealsImageTapped;
            this.AdDealsImage.GestureRecognizers.Add(tapGesture2);

            this.PopupGrid = new Grid
            {
                Children =
                {
                    this.AdDealsImageFrame,
                    this.CloseButtonFrame,
                },
                Margin = 2,
                WidthRequest = adImageWidth,
                HeightRequest = adImageHeight,
                BackgroundColor = Color.Transparent,
            };

            this.Content = this.PopupGrid;

            this.AdDealsImage.SizeChanged += AdDealsImage_SizeChanged;
            this.BackgroundColor = Color.FromHex("#C0000000");
        }

        private void AdDealsImage_SizeChanged(object sender, EventArgs e)
        {
            var image = (sender as Image);
            if (image.Width > 0 && image.Height > 0)
            {
                this.CloseButtonFrame.TranslationX = image.Width / 2 - CloseButtonWidth / 2;
                this.CloseButtonFrame.TranslationY = -1 * image.Height / 2 + CloseButtonHeight / 2;
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
