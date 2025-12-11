using EatWork.Mobile.Contants;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Utils
{
    /// <summary>
    /// https://stackoverflow.com/questions/48648585/hide-row-if-data-is-empty-in-listview
    /// </summary>
    public class ImageAvatarConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retValue = Syncfusion.XForms.AvatarView.ContentType.Custom;

            //if (string.IsNullOrWhiteSpace(value.ToString()))
            //    retValue = Syncfusion.XForms.AvatarView.ContentType.AvatarCharacter;

            if (value == null)
                retValue = Syncfusion.XForms.AvatarView.ContentType.AvatarCharacter;

            return retValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class IconByTrasanctionTypeConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retValue = Constants.HourIcon;

            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                var id = (Int64)value;

                if (id == TransactionType.Loan)
                    retValue = Constants.AmountIcon;
                else if (id == TransactionType.Document)
                    retValue = Constants.FileIcon;
            }

            return retValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class IconByTrasanctionType2Converter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retValue = Constants.TimeIcon;

            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                var id = (Int64)value;

                if (id == TransactionType.ChangeRestDay)
                    retValue = Constants.DateIcon;
            }

            return retValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class TransactionTypeBoolConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retValue = false;

            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                var id = (Int64)value;

                if (id == TransactionType.Document)
                    retValue = true;
                else
                    retValue = false;
            }

            return retValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}