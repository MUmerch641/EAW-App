using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using Android.Views; // 🔥 1. Ye naya namespace add karo

namespace MauiHybridApp;

[Activity(
    Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true, 
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
    WindowSoftInputMode = SoftInput.AdjustResize
)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

#if DEBUG
		try
		{
			global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
		}
		catch (Exception)
		{
		}
#endif

		// Configure WebView settings
		try
		{
            // Ye code thora risky hai (New WebView create kar ke destroy karna)
            // Lekin agar pehle se chal raha tha to rehne do.
            // Agar crash ho to bata dena, isay hata denge.
			var webView = new Android.Webkit.WebView(this);
			var settings = webView.Settings;
			settings.JavaScriptEnabled = true;
			settings.DomStorageEnabled = true;
			settings.AllowFileAccess = true;
			settings.AllowFileAccessFromFileURLs = true;
			settings.AllowUniversalAccessFromFileURLs = true;
			settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
			webView.Destroy(); 
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"WebView configuration error: {ex.Message}");
		}
	}
}