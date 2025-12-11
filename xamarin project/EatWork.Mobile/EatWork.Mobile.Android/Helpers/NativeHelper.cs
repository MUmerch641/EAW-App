using Android.App;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Droid.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(NativeHelper))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class NativeHelper : INativeHelper
    {
        public void CloseApp()
        {
            var activity = (Activity)Android.App.Application.Context;
            activity.FinishAffinity();
        }
    }
}