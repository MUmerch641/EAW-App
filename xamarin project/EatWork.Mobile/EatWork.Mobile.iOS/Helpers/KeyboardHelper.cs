using EatWork.Mobile.Contracts;
using EatWork.Mobile.iOS.Helpers;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(KeyboardHelper))]

namespace EatWork.Mobile.iOS.Helpers
{
    /// <summary>
    /// https://forums.xamarin.com/discussion/comment/186777
    /// </summary>
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
        }
    }
}