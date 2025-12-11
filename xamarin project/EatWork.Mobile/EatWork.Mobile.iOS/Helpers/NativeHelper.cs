using EatWork.Mobile.Contracts;
using EatWork.Mobile.iOS.Helpers;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(NativeHelper))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class NativeHelper : INativeHelper
    {
        public void CloseApp()
        {
            Thread.CurrentThread.Abort();
        }
    }
}