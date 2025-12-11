using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Utils
{
    public class CustomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object eventArgs = null;

            if (value is Syncfusion.ListView.XForms.ItemTappedEventArgs)
                eventArgs = value as Syncfusion.ListView.XForms.ItemTappedEventArgs;
            else if (value is Syncfusion.ListView.XForms.ListViewLoadedEventArgs)
                eventArgs = parameter;
            else if (value is Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs)
                eventArgs = value as Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs;
            else if (value is Syncfusion.ListView.XForms.SwipingEventArgs)
                eventArgs = value as Syncfusion.ListView.XForms.SwipingEventArgs;
            else if (value is Syncfusion.XForms.ComboBox.SelectionChangedEventArgs)
                eventArgs = value as Syncfusion.XForms.ComboBox.SelectionChangedEventArgs;
            else if (value is Syncfusion.XForms.Buttons.SelectionChangedEventArgs)
                eventArgs = value as Syncfusion.XForms.Buttons.SelectionChangedEventArgs;
            else if (value is DateChangedEventArgs)
                eventArgs = value as DateChangedEventArgs;
            else if (value is Syncfusion.XForms.Buttons.StateChangedEventArgs)
                eventArgs = value as Syncfusion.XForms.Buttons.StateChangedEventArgs;
            else if (value is Syncfusion.XForms.Buttons.StateChangedEventArgs)
                eventArgs = value as Syncfusion.XForms.Buttons.StateChangedEventArgs;
            else if (value is Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs)
                eventArgs = value as Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs;
            else if (value is System.ComponentModel.PropertyChangedEventArgs)
                eventArgs = value as System.ComponentModel.PropertyChangedEventArgs;
            else if (value is Syncfusion.SfRating.XForms.ValueEventArgs)
                eventArgs = value as Syncfusion.SfRating.XForms.ValueEventArgs;
            return eventArgs;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FlexLayoutContentConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retValue = FlexJustify.SpaceEvenly;

            if (((bool)value))
            {
                retValue = FlexJustify.SpaceBetween;
            }

            return retValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class FlexLayoutCenterContentConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retValue = FlexJustify.Center;

            if (((bool)value))
            {
                retValue = FlexJustify.SpaceBetween;
            }

            return retValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class SelectionModeToVisbileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((SelectionMode)value == SelectionMode.Multiple)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}