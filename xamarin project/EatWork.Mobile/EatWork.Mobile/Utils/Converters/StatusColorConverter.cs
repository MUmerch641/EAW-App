using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Utils
{
    public class StatusColorConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var returnValue = Contants.Color.Draft;

            if (value == null)
                return returnValue;

            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                switch (value.ToString())
                {
                    case Contants.RequestStatus.Disapproved:
                        returnValue = Contants.Color.Disapproved;
                        break;

                    case Contants.RequestStatus.Approved:
                        returnValue = Contants.Color.Approved;
                        break;

                    case Contants.RequestStatus.Assessed:
                        returnValue = Contants.Color.Approved;
                        break;

                    case Contants.RequestStatus.Completed:
                        returnValue = Contants.Color.Approved;
                        break;

                    case Contants.RequestStatus.Cancelled:
                        returnValue = Contants.Color.Cancelled;
                        break;

                    case Contants.RequestStatus.Draft:
                        returnValue = Contants.Color.Draft;
                        break;

                    case "---":
                        returnValue = Contants.Color.Draft;
                        break;

                    case Contants.RequestStatus.ForAssessment:
                        returnValue = Contants.Color.ForApproval;
                        break;

                    default:
                        returnValue = Contants.Color.ForApproval;
                        break;
                }
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class ActionTypeColorConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var returnValue = Contants.Color.Draft;
            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                switch (value.ToString())
                {
                    case Contants.ActionType.Disapprove:
                        returnValue = Contants.Color.Disapproved;
                        break;

                    case Contants.ActionType.Approve:
                        returnValue = Contants.Color.Approved;
                        break;

                    case Contants.ActionType.ForApproval:
                        returnValue = Contants.Color.ForApproval;
                        break;

                    case Contants.ActionType.PartiallyApprove:
                        returnValue = Contants.Color.ForApproval;
                        break;

                    case Contants.ActionType.Cancel:
                        returnValue = Contants.Color.Cancelled;
                        break;

                    case Contants.ActionType.Draft:
                        returnValue = Contants.Color.Draft;
                        break;

                    default:
                        returnValue = Contants.Color.Draft;
                        break;
                }
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class BooleanToColorConverter : IValueConverter
    {
        /// <summary>
        /// This method is used to convert the bool to color.
        /// </summary>
        /// <param name="value">Gets the value.</param>
        /// <param name="targetType">Gets the target type.</param>
        /// <param name="parameter">Gets the parameter.</param>
        /// <param name="culture">Gets the culture.</param>
        /// <returns>Returns the color.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return Color.Default;
            }

            switch (parameter.ToString())
            {
                case "0" when (bool)value:
                    return Color.FromRgba(255, 255, 255, 0.6);

                case "1" when (bool)value:
                    return Color.FromHex("#FF4A4A");

                case "2" when (bool)value:
                    return Color.FromHex("#FF4A4A");

                case "2":
                    return Color.FromHex("#ced2d9");

                case "3" when (bool)value:
                    return Color.FromHex("#959eac");

                case "3":
                    return Color.FromHex("#ced2d9");

                case "4" when (bool)value:
                    Application.Current.Resources.TryGetValue("PrimaryColor", out var retVal);
                    return (Color)retVal;

                case "4":
                    Application.Current.Resources.TryGetValue("Gray-600", out var outVal);
                    return (Color)outVal;

                case "5" when (bool)value:
                    Application.Current.Resources.TryGetValue("Green", out var retGreen);
                    return (Color)retGreen;

                case "5":
                    Application.Current.Resources.TryGetValue("Red", out var retRed);
                    return (Color)retRed;

                case "6" when (bool)value:
                    Application.Current.Resources.TryGetValue("Gray-300", out var gray300);
                    return (Color)gray300;

                case "6":
                    Application.Current.Resources.TryGetValue("Secondary", out var secondary);
                    return (Color)secondary;

                case "7" when !(bool)value:
                    Application.Current.Resources.TryGetValue("Gray-100", out var gray100);
                    return (Color)gray100;

                case "8" when (bool)value:
                    Application.Current.Resources.TryGetValue("PrimaryColor", out var primary);
                    return (Color)primary;

                case "8":
                    Application.Current.Resources.TryGetValue("Gray-White", out var graywhite);
                    return (Color)graywhite;

                default:
                    return Color.Transparent;
            }
        }

        /// <summary>
        /// This method is used to convert the color to bool.
        /// </summary>
        /// <param name="value">Gets the value.</param>
        /// <param name="targetType">Gets the target type.</param>
        /// <param name="parameter">Gets the parameter.</param>
        /// <param name="culture">Gets the culture.</param>
        /// <returns>Returns the string.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EventDateColorConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var returnValue = Contants.Color.DateDefaultColor;

            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                returnValue = Contants.Color.DateEventColor;
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class BackgroundColorOpacityConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double opacity = 0.0;

            if ((bool)value)
            {
                opacity = 0.28;
            }

            return opacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}