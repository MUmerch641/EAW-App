using Syncfusion.XForms.TextInputLayout;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EatWork.Mobile.Utils.NullableTimePicker), typeof(EatWork.Mobile.iOS.Helpers.NullableTimePickerRenderer))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class NullableTimePickerRenderer : TimePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);

            TryShowEmptyState();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
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
                    e.PropertyName == EatWork.Mobile.Utils.NullableTimePicker.EmptyStateTextProperty.PropertyName ||
                    e.PropertyName == EatWork.Mobile.Utils.NullableTimePicker.TimeProperty.PropertyName)
                {
                    TryShowEmptyState();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
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