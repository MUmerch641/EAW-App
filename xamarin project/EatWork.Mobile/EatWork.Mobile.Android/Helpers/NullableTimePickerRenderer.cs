using Android.Content;
using Syncfusion.XForms.TextInputLayout;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EatWork.Mobile.Utils.NullableTimePicker), typeof(EatWork.Mobile.Droid.Helpers.NullableTimePickerRenderer))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class NullableTimePickerRenderer : TimePickerRenderer
    {
        public NullableTimePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
        {
            base.OnElementChanged(e);

            TryShowEmptyState();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            /*
            if (e.PropertyName == Xamarin.Forms.TimePicker.TimeProperty.PropertyName || e.PropertyName == Xamarin.Forms.TimePicker.FormatProperty.PropertyName)
            {
                var entry = (EatWork.Mobile.Utils.NullableTimePicker)this.Element;

                if (this.Element.Format == entry.EmptyStateText)
                {
                    this.Control.Text = entry.EmptyStateText;
                    return;
                }
            }
            */

            // Check if the property we are updating is the format property
            if (e.PropertyName == Xamarin.Forms.TimePicker.TimeProperty.PropertyName || e.PropertyName == Xamarin.Forms.TimePicker.FormatProperty.PropertyName)
            {
                var entry = (EatWork.Mobile.Utils.NullableTimePicker)this.Element;
                var parent = ((this.Element.Parent as Grid).Parent as StackLayout).Parent as SfTextInputLayout;
                if (parent != null)
                {
                    MethodInfo method = parent.GetType().GetMethod("UpdateText", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (entry.NullableTime == null)
                    {
                        method.Invoke(parent, new object[] { string.Empty, false });
                    }
                    else
                    {
                        method.Invoke(parent, new object[] { entry.Time.ToString(), false });
                    }
                    // If we are updating the format to the placeholder then just update the text and return
                    if (this.Element.Format == entry.EmptyStateText)
                    {
                        this.Control.Text = entry.EmptyStateText;
                        return;
                    }
                }
            }
            else if (e.PropertyName == EatWork.Mobile.Utils.NullableTimePicker.NullableTimeProperty.PropertyName)
            {
                var entry = (EatWork.Mobile.Utils.NullableTimePicker)this.Element;
                if (entry.NullableTime == null)
                {
                    var parent = ((this.Element.Parent as Grid).Parent as StackLayout).Parent as SfTextInputLayout;
                    if (parent != null)
                    {
                        MethodInfo method = parent.GetType().GetMethod("UpdateText", BindingFlags.NonPublic | BindingFlags.Instance);
                        method.Invoke(parent, new object[] { string.Empty, false });
                    }
                }
                else
                {
                    entry.Time = entry.NullableTime.GetValueOrDefault();
                }
            }

            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == EatWork.Mobile.Utils.NullableTimePicker.NullableTimeProperty.PropertyName ||
                e.PropertyName == EatWork.Mobile.Utils.NullableTimePicker.EmptyStateTextProperty.PropertyName)
            {
                TryShowEmptyState();
            }
        }

        private void TryShowEmptyState()
        {
            var el = Element as EatWork.Mobile.Utils.NullableTimePicker;
            if (el != null)
            {
                if (el.NullableTime == null)
                {
                    Control.Text = el.EmptyStateText;
                }
            }
        }
    }
}