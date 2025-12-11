using EatWork.Mobile.iOS.Helpers;
using EatWork.Mobile.Utils.Effects;
using System;
using System.Diagnostics;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("EatWork")]
[assembly: ExportEffect(typeof(iOSSafeAreaPaddingEffect), nameof(SafeAreaPaddingEffect))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class iOSSafeAreaPaddingEffect : PlatformEffect
    {
        private Thickness _padding;

        protected override void OnAttached()
        {
            if (Element is Layout element)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    _padding = element.Padding;

                    // Check if there's a valid window
                    var window = UIApplication.SharedApplication.Windows.FirstOrDefault();
                    if (window != null && window.SafeAreaInsets != UIEdgeInsets.Zero)
                    {
                        var insets = window.SafeAreaInsets;

                        if (insets.Top > 0) // We have a notch
                        {
                            element.Padding = new Thickness(
                                _padding.Left + insets.Left,
                                _padding.Top + insets.Top,
                                _padding.Right + insets.Right,
                                _padding.Bottom);
                            return;
                        }
                    }
                    else
                    {
                        // No valid window found; log the issue and apply default padding
                        Debug.WriteLine("Warning: No valid window found or SafeAreaInsets unavailable. Applying default padding.");
                        element.Padding = new Thickness(_padding.Left, _padding.Top + 20, _padding.Right, _padding.Bottom);
                    }
                }
                else
                {
                    // iOS versions < 11.0 or when SafeAreaInsets aren't used
                    element.Padding = new Thickness(_padding.Left, _padding.Top + 20, _padding.Right, _padding.Bottom);
                }
            }

            /*
            if (Element is Layout element)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    _padding = element.Padding;

                    // Check if there's a valid window
                    var window = UIApplication.SharedApplication.Windows.FirstOrDefault();
                    if (window != null && window.SafeAreaInsets != UIEdgeInsets.Zero)
                    {
                        var insets = window.SafeAreaInsets;

                        if (insets.Top > 0) // We have a notch
                        {
                            element.Padding = new Thickness(
                                _padding.Left + insets.Left,
                                _padding.Top + insets.Top,
                                _padding.Right + insets.Right,
                                _padding.Bottom);
                            return;
                        }
                    }
                    else
                    {
                        // No valid window found; log the issue and apply default padding
                        Debug.WriteLine("Warning: No valid window found or SafeAreaInsets unavailable. Applying default padding.");
                        element.Padding = new Thickness(_padding.Left, _padding.Top + 20, _padding.Right, _padding.Bottom);
                    }
                }
                else
                {
                    // iOS versions < 11.0 or when SafeAreaInsets aren't used
                    element.Padding = new Thickness(_padding.Left, _padding.Top + 20, _padding.Right, _padding.Bottom);
                }
            }

            /*
            if (Element is Layout element)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    _padding = element.Padding;
                    var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets; // Can't use KeyWindow this early
                    if (insets.Top > 0) // We have a notch
                    {
                        element.Padding = new Thickness(_padding.Left + insets.Left, _padding.Top + insets.Top, _padding.Right + insets.Right, _padding.Bottom);
                        return;
                    }
                }
                // Uses a default Padding of 20. Could use an property to modify if you wanted.
                element.Padding = new Thickness(_padding.Left, _padding.Top + 20, _padding.Right, _padding.Bottom);
            }
            */
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _padding;
            }
        }
    }
}