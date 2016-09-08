namespace AdDealsNetworkLib
{
    using System;
    using Xamarin.Forms;

    public class HybridWebView : View
	{
		Action<string> action;

        public static readonly BindableProperty MethodToInvokeProperty = BindableProperty.Create(
            propertyName: "MethodToInvoke",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public string MethodToInvoke
        {
            get { return (string)GetValue(MethodToInvokeProperty); }
            set { SetValue(MethodToInvokeProperty, value); }
        }

        public static readonly BindableProperty UriProperty = BindableProperty.Create (
			propertyName: "Uri",
			returnType: typeof(string),
			declaringType: typeof(HybridWebView),
			defaultValue: default(string));
		
		public string Uri {
			get { return (string)GetValue (UriProperty); }
			set { SetValue (UriProperty, value); }
		}

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            propertyName: "Source",
            returnType: typeof(HtmlWebViewSource),
            declaringType: typeof(HybridWebView),
            defaultValue: default(HtmlWebViewSource));

        public HtmlWebViewSource Source
        {
            get { return (HtmlWebViewSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public void RegisterAction (Action<string> callback)
		{
			action = callback;
		}

		public void Cleanup ()
		{
			action = null;
		}

		public void InvokeAction (string data)
		{
			if (action == null || data == null) {
				return;
			}
			action.Invoke (data);
		}
	}
}
