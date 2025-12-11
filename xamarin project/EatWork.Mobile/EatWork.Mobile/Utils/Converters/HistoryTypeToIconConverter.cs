using EatWork.Mobile.Contants;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Utils
{
    public class HistoryTypeToIconConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retValue = "fas-exclamation-triangle";

            if (value != null)
            {
                var typeId = (long)value;

                if (typeId == HistoryType.CUSTOM_FOR_DISPLAY)
                {
                    retValue = "fab-font-awesome-flag";
                }
                else if (typeId == HistoryType.UNROUTED_SUBMITTED_TRANSACTION ||
                    typeId == HistoryType.UNROUTED_APPROVED_TRANSACTION ||
                    typeId == HistoryType.UNROUTED_TRANSACTION)
                {
                    retValue = "fas-exclamation-triangle";
                }
                else if (typeId == HistoryType.REQUESTOR_TO_PARTIAL_APPROVER)
                {
                    retValue = "fas-star";
                }
                else if (typeId == HistoryType.PARTIAL_TO_NEXT_APPROVER ||
                         typeId == HistoryType.REROUTED_TRANSACTION)
                {
                    retValue = "fas-arrow-right";
                }
                else if (typeId == HistoryType.IMPORTED_TRANSACTION)
                {
                    retValue = "fas-upload";
                }
                else if (typeId == HistoryType.ADDED_APPROVER ||
                         typeId == HistoryType.REMOVED_APPROVER)
                {
                    retValue = "fas-info";
                }
                else
                {
                    var label = parameter as Label;
                    var param = long.Parse(label.Text);

                    if (param == ActionTypeId.Cancel ||
                       param == ActionTypeId.Disapprove ||
                       param == ActionTypeId.Suspend ||
                       param == ActionTypeId.MarkAsActive)
                    {
                        retValue = "fas-exclamation-triangle";
                    }
                    else if (param == ActionTypeId.Resume)
                    {
                        retValue = "fas-sync";
                    }
                    else
                    {
                        retValue = "fas-check";
                    }
                }
            }

            return retValue;
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

    public class HistoryTypeToColorConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retValue = Contants.Color.Warning;

            if (value != null)
            {
                var typeId = (long)value;

                if (typeId == HistoryType.CUSTOM_FOR_DISPLAY)
                {
                    retValue = Contants.Color.NavigationPrimary;
                }
                else if (typeId == HistoryType.UNROUTED_SUBMITTED_TRANSACTION ||
                    typeId == HistoryType.UNROUTED_APPROVED_TRANSACTION ||
                    typeId == HistoryType.UNROUTED_TRANSACTION)
                {
                    retValue = Contants.Color.Warning;
                }
                else if (typeId == HistoryType.REQUESTOR_TO_PARTIAL_APPROVER)
                {
                    retValue = Contants.Color.Warning;
                }
                else if (typeId == HistoryType.PARTIAL_TO_NEXT_APPROVER ||
                         typeId == HistoryType.REROUTED_TRANSACTION)
                {
                    retValue = Contants.Color.Info;
                }
                else if (typeId == HistoryType.IMPORTED_TRANSACTION)
                {
                    retValue = Contants.Color.Info;
                }
                else if (typeId == HistoryType.ADDED_APPROVER ||
                         typeId == HistoryType.REMOVED_APPROVER)
                {
                    retValue = Contants.Color.Info;
                }
                else
                {
                    var label = parameter as Label;
                    var param = long.Parse(label.Text);

                    if (param == ActionTypeId.Cancel ||
                       param == ActionTypeId.Disapprove ||
                       param == ActionTypeId.Suspend ||
                       param == ActionTypeId.MarkAsActive)
                    {
                        retValue = Contants.Color.Warning;
                    }
                    else if (param == ActionTypeId.Resume)
                    {
                        retValue = Contants.Color.Info;
                    }
                    else
                    {
                        retValue = Contants.Color.Approved;
                    }
                }
            }

            return retValue;
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