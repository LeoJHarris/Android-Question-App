using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace Android_Question_App
{
    [Activity(Label = "SidebarActivity")]
    public class SidebarActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var sidebarUrl = Intent.Extras.GetString("sidebarUrl");
            var webView = new WebView(this);
            webView.SetWebViewClient(new WebViewClient());
            WebSettings webSettings = webView.Settings;
            webSettings.JavaScriptEnabled = true;
            webSettings.UserAgentString = "Mozilla/5.0 (Linux; Android 4.4; Nexus 5 Build/_BuildID_) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/30.0.0.0 Mobile Safari/537.36";
            webSettings.SetAppCacheEnabled(true);

            var metrics = Resources.DisplayMetrics;
            AddContentView(webView, new ViewGroup.LayoutParams(metrics.WidthPixels, metrics.HeightPixels));
            webView.LoadUrl(sidebarUrl);
        }
    }
}