using Android.App;
using Android.Views;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Droid.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarHelper))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class StatusBarHelper : IStatusBar
    {
        private WindowManagerFlags _originalFlags;

        public StatusBarHelper()
        {
        }

        public void HideStatusBar()
        {
            var activity = (Activity)Android.App.Application.Context;
            var attrs = activity.Window.Attributes;
            _originalFlags = attrs.Flags;
            attrs.Flags |= Android.Views.WindowManagerFlags.Fullscreen;
            activity.Window.Attributes = attrs;
        }

        public void ShowStatusBar()
        {
            var activity = (Activity)Android.App.Application.Context;
            var attrs = activity.Window.Attributes;
            attrs.Flags = _originalFlags;
            activity.Window.Attributes = attrs;
        }
    }
}