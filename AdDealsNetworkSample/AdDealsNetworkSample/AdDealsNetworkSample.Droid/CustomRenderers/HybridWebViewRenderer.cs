using AdDealsNetworkLib;
using AdDealsNetworkSample.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using androidWebKit = Android.Webkit;

[assembly: ExportRenderer (typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace AdDealsNetworkSample.Droid
{
	public class HybridWebViewRenderer : ViewRenderer<HybridWebView, androidWebKit.WebView>
	{
        JSBridge jsBridge;
        androidWebKit.WebView webView;
        string methodToInvoke = string.Empty;
        const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";

		protected override void OnElementChanged (ElementChangedEventArgs<HybridWebView> e)
		{
			base.OnElementChanged (e);

			if (Control == null) {
				webView = new androidWebKit.WebView (Xamarin.Forms.Forms.Context);
				webView.Settings.JavaScriptEnabled = true;
				SetNativeControl (webView);
			}
			if (e.OldElement != null) {
				Control.RemoveJavascriptInterface ("jsBridge");
				var hybridWebView = e.OldElement as HybridWebView;
				hybridWebView.Cleanup ();
			}
			if (e.NewElement != null) {
                jsBridge = new JSBridge(this);
                Control.AddJavascriptInterface (jsBridge, "jsBridge");
                methodToInvoke = Element.MethodToInvoke;
                InjectJS (JavaScriptFunction);
                Control.LoadUrl(string.Format("file:///android_asset/Content/{0}", Element.Uri));
            }
		}

		void InjectJS (string script)
		{
			if (Control != null) {
				Control.LoadUrl (string.Format ("javascript: {0}", script));
            }
        }
    }
}
