using EatWork.Mobile.Contracts;
using EatWork.Mobile.iOS.Helpers;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarHelper))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class StatusBarHelper : IStatusBar
    {
        public StatusBarHelper()
        {
        }

        public void HideStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = true;
        }

        public void ShowStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = false;
        }
    }
}