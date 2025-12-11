using Android.Content;
using Android.Graphics;
using EatWork.Mobile.Droid.Helpers;
using EatWork.Mobile.Utils;
using Plugin.Iconize;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(CustomIconNavigationPage), typeof(NavigationPageRenderer))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class NavigationPageRenderer : IconNavigationRenderer
    {
        private Android.Support.V7.Widget.Toolbar _toolbar;

        public NavigationPageRenderer(Context context) : base(context)
        {
        }

        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);

            if (child.GetType() == typeof(Android.Support.V7.Widget.Toolbar))
            {
                _toolbar = (Android.Support.V7.Widget.Toolbar)child;
                _toolbar.ChildViewAdded += Toolbar_ChildViewAdded;
            }
        }

        private void Toolbar_ChildViewAdded(object sender, ChildViewAddedEventArgs e)
        {
            var view = e.Child.GetType();
            var textSize = 18;
            var spaceFont = Typeface.CreateFromAsset(Android.App.Application.Context.ApplicationContext.Assets, "Montserrat-Medium.ttf");

            if (view == typeof(Android.Widget.TextView))
            {
                var textView = (Android.Widget.TextView)e.Child;
                textView.Typeface = spaceFont;
                textView.TextSize = textSize;
                _toolbar.ChildViewAdded -= Toolbar_ChildViewAdded;
            }

            if (view == typeof(Android.Support.V7.Widget.AppCompatTextView))
            {
                var textView = (Android.Support.V7.Widget.AppCompatTextView)e.Child;
                textView.Typeface = spaceFont;
                textView.TextSize = textSize;
                _toolbar.ChildViewAdded -= Toolbar_ChildViewAdded;
            }
        }
    }
}