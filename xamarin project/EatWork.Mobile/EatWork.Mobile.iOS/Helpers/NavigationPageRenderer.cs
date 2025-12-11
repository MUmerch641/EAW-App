using EatWork.Mobile.iOS.Helpers;
using EatWork.Mobile.Utils;
using Plugin.Iconize;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomIconNavigationPage), typeof(NavigationPageRenderer))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class NavigationPageRenderer : IconNavigationRenderer
    {
        public NavigationPageRenderer()
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
                {
                    Font = UIFont.FromName("Montserrat-Medium", 18),
                    TextColor = UIColor.White,
                    TextShadowColor = UIColor.Clear,
                });

                UINavigationBar.Appearance.BackgroundColor = UIColor.FromRGB(47, 114, 228);
                UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(47, 114, 228);
                UINavigationBar.Appearance.BarStyle = UIBarStyle.Black;
                UINavigationBar.Appearance.ShadowImage = new UIImage();
                UINavigationBar.Appearance.TintColor = UIColor.White;
				
                //==Override Dark Theme
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
                }
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();           

			//==Override Dark Theme
			if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
			{
				OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
			}
        }
    }
}