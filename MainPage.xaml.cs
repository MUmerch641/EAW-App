using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MauiHybridApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Configure Blazor WebView for better Android compatibility
        blazorWebView.BlazorWebViewInitializing += (sender, e) =>
        {
#if ANDROID
            // Configure Android WebView settings after the handler is created
            blazorWebView.HandlerChanged += (s, args) =>
            {
                if (blazorWebView.Handler?.PlatformView is Android.Webkit.WebView nativeWebView)
                {
                    var settings = nativeWebView.Settings;
                    settings.JavaScriptEnabled = true;
                    settings.DomStorageEnabled = true;
                    settings.AllowFileAccess = true;
                    settings.AllowFileAccessFromFileURLs = true;
                    settings.AllowUniversalAccessFromFileURLs = true;
                    settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
                    settings.SetSupportMultipleWindows(false);
                    settings.SetSupportZoom(false);

                    // Set user agent to help with compatibility
                    settings.UserAgentString = settings.UserAgentString + " MauiBlazorHybrid";

                    // Enable debugging in debug mode
#if DEBUG
                    Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
#endif
                }
            };
#endif
        };

        blazorWebView.BlazorWebViewInitialized += (sender, e) =>
        {
            // Blazor WebView is ready
            System.Diagnostics.Debug.WriteLine("Blazor WebView initialized successfully");
        };
    }
}

