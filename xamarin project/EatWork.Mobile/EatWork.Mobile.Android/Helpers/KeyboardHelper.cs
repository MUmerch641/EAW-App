using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Droid.Helpers;
using Plugin.CurrentActivity;

[assembly: Xamarin.Forms.Dependency(typeof(KeyboardHelper))]

namespace EatWork.Mobile.Droid.Helpers
{
    /// <summary>
    /// https://forums.xamarin.com/discussion/comment/186777
    /// https://theconfuzedsourcecode.wordpress.com/2017/04/30/forcefully-dismissing-keyboard-in-xamarin-forms/
    /// </summary>
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            InputMethodManager imm = InputMethodManager.FromContext(CrossCurrentActivity.Current.Activity.ApplicationContext);

            imm.HideSoftInputFromWindow(CrossCurrentActivity.Current.Activity.Window.DecorView.WindowToken
                , HideSoftInputFlags.NotAlways);

            //var context = Android.App.Application.Context;
            //var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            //if (inputMethodManager != null && context is Activity)
            //{
            //    var activity = context as Activity;
            //    var token = activity.CurrentFocus?.WindowToken;
            //    inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
            //    activity.Window.DecorView.ClearFocus();
            //}
        }
    }
}