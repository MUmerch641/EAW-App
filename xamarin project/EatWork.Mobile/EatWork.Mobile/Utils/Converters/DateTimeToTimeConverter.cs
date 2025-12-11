using EatWork.Mobile.Contants;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Utils
{
    public class DateTimeToTimeConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return ((DateTime)value).TimeOfDay;
            else
                return null;
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

    public class DateTimeToStringConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((DateTime)value == Constants.NullDate)
                {
                    return string.Empty;
                }
                else
                {
                    return ((DateTime)value).ToString(FormHelper.DateFormat);
                }
            }
            else
                return string.Empty;
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

    public class FullDateTimeToStringConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((DateTime)value == Constants.NullDate)
                {
                    return string.Empty;
                }
                else
                {
                    return ((DateTime)value).ToString(Constants.DateFormatMMDDYYYYHHMMTT);
                }
            }
            else
                return string.Empty;
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

    public class WFTransactionHistoryDateConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((DateTime)value == Constants.NullDate)
                {
                    return string.Empty;
                }
                else
                {
                    var date = ((DateTime)value);

                    if (date.Date == (Constants.NullDate).Date)
                    {
                        return $"Today at {date.ToString(Constants.TimeFormatHHMMTT)}";
                    }
                    else
                    {
                        return $"{date.ToString(FormHelper.DateFormat)} at {date.ToString(Constants.TimeFormatHHMMTT)}";
                    }
                }
            }
            else
                return string.Empty;
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

    public class DateTimeToBoolConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((DateTime)value == Constants.NullDate)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                return string.Empty;
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
}