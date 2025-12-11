using Android.Content;
using EatWork.Mobile.Contants;
using Syncfusion.XForms.TextInputLayout;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EatWork.Mobile.Utils.NullableDatePicker), typeof(EatWork.Mobile.Droid.Helpers.NullableDatePickerRenderer))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class NullableDatePickerRenderer : DatePickerRenderer
    {
        public NullableDatePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            TryShowEmptyState();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            /*
            if (e.PropertyName == Xamarin.Forms.DatePicker.DateProperty.PropertyName || e.PropertyName == Xamarin.Forms.DatePicker.FormatProperty.PropertyName)
            {
                var entry = (EatWork.Mobile.Utils.NullableDatePicker)this.Element;

                if (this.Element.Format == entry.EmptyStateText)
                {
                    this.Control.Text = entry.EmptyStateText;
                    return;
                }
            }
            */
            // Check if the property we are updating is the format property
            if (e.PropertyName == Xamarin.Forms.DatePicker.DateProperty.PropertyName || e.PropertyName == Xamarin.Forms.DatePicker.FormatProperty.PropertyName)
            {
                var entry = (EatWork.Mobile.Utils.NullableDatePicker)this.Element;
                var parent = ((this.Element.Parent as Grid).Parent as StackLayout).Parent as SfTextInputLayout;

                if (parent != null)
                {
                    MethodInfo method = parent.GetType().GetMethod("UpdateText", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (entry.NullableDate == null || entry.NullableDate <= Constants.NullDate)
                    {
                        method.Invoke(parent, new object[] { string.Empty, false });
                    }
                    else
                    {
                        method.Invoke(parent, new object[] { entry.Date.ToString(), false });
                    }
                    // If we are updating the format to the placeholder then just update the text and return
                    if (this.Element.Format == entry.EmptyStateText)
                    {
                        this.Control.Text = entry.EmptyStateText;
                        return;
                    }
                }
            }
            else if (e.PropertyName == EatWork.Mobile.Utils.NullableDatePicker.NullableDateProperty.PropertyName)
            {
                var entry = (EatWork.Mobile.Utils.NullableDatePicker)this.Element;
                if (entry.NullableDate == null || entry.NullableDate <= Constants.NullDate)
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
                    entry.Date = entry.NullableDate.GetValueOrDefault();
                }
            }

            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == EatWork.Mobile.Utils.NullableDatePicker.NullableDateProperty.PropertyName ||
                e.PropertyName == EatWork.Mobile.Utils.NullableDatePicker.EmptyStateTextProperty.PropertyName)
            {
                TryShowEmptyState();
            }
        }

        private void TryShowEmptyState()
        {
            var el = Element as EatWork.Mobile.Utils.NullableDatePicker;
            if (el != null)
            {
                if (el.NullableDate == null ||
                    el.NullableDate.Value <= Constants.NullDate)
                {
                    Control.Text = el.EmptyStateText;
                }
            }
        }
    }
}