using EatWork.Mobile.Contants;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace EatWork.Mobile.Utils
{
    public static class DateHelper
    {
        public static string ConcatDate(this string input, DateTime date1, DateTime date2)
        {
            var dt1 = string.Empty;
            var dt2 = string.Empty;
            var separator = string.Empty;

            date1 = Convert.ToDateTime(date1.ToString(FormHelper.DateFormat));
            date2 = Convert.ToDateTime(date2.ToString(FormHelper.DateFormat));

            if (date1 > Constants.NullDate)
                dt1 = date1.ToString(FormHelper.DateFormat);

            if (date2 > Constants.NullDate)
            {
                if (date2 > date1)
                {
                    separator = " - ";
                    dt2 = date2.ToString(FormHelper.DateFormat);
                }
            }

            return input = $"{dt1}{separator}{dt2}";
        }

        public static string ConcatDateTime(this string input, DateTime baseDate, DateTime date1, DateTime date2)
        {
            if (baseDate.Date > Constants.NullDate)
            {
                var dt1 = string.Empty;
                var dt2 = string.Empty;
                var separator = string.Empty;
                baseDate = Convert.ToDateTime(baseDate.ToString(Constants.DateFormatMMDDYYYYHHMMTT));
                date1 = Convert.ToDateTime(date1.ToString(Constants.DateFormatMMDDYYYYHHMMTT));
                date2 = Convert.ToDateTime(date2.ToString(Constants.DateFormatMMDDYYYYHHMMTT));

                if (date1 > Constants.NullDate)
                {
                    if (date1.Date == Constants.NullDate.Date)
                        date1 = baseDate + date1.TimeOfDay;

                    dt1 = (date1.Date == baseDate.Date ? date1.ToString(Constants.TimeFormatHHMMTT) : date1.ToString(Constants.DateFormatMMDDYYYYHHMMTT));
                }

                if (date2 > Constants.NullDate)
                {
                    if (date2.Date == Constants.NullDate.Date)
                        date2 = baseDate + date2.TimeOfDay;

                    if (date2.Date == Constants.NullDate2.Date)
                        date2 = baseDate.AddDays(1) + date2.TimeOfDay;

                    separator = " - ";
                    dt2 = (date2.Date == baseDate.Date ? date2.ToString(Constants.TimeFormatHHMMTT) : date2.ToString(Constants.DateFormatMMDDYYYYHHMMTT));
                }

                input = $"{dt1}{separator}{dt2}";
            }

            return input;
        }

        public static DateTime DuplicateDate(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static DateTime AdjustTime(this DateTime dateTime, DateTime timeSpan)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, timeSpan.Hour, timeSpan.Minute, timeSpan.Second);
        }

        public static DateTime StringToDate(this string input, string format = "")
        {
            var retVal = new DateTime();
            format = (string.IsNullOrWhiteSpace(format) ? Constants.DateFormatMMDDYYYYHHMMTT : format);

            if (!string.IsNullOrWhiteSpace(input))
            {
                retVal = DateTime.ParseExact(input, format, CultureInfo.InvariantCulture);
            }

            return retVal;
        }

        //used for custom datetime picker
        public static ObservableCollection<object> SetObjectValue(ObservableCollection<object> item, DateTime? date = null)
        {
            var dateValue = Convert.ToDateTime((date == null ? DateTime.Now : date));

            item = new ObservableCollection<object>
            {
                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateValue.Date.Month).Substring(0, 3),
                ////Select today dates
                //dateValue.Year.ToString(),
            };

            //date
            if (dateValue.Date.Day < 10)
                item.Add("0" + dateValue.Date.Day);
            else
                item.Add(dateValue.Date.Day.ToString());

            //year
            item.Add(dateValue.Year.ToString());

            //hour
            if (dateValue.Hour < 10)
                item.Add("0" + dateValue.Hour.ToString());
            else
            {
                if (dateValue.Hour <= 12)
                    item.Add(dateValue.Hour.ToString());
                else
                {
                    var hour = (dateValue.Hour - 12);

                    if (hour < 10)
                        item.Add("0" + hour.ToString());
                    else
                        item.Add(hour.ToString());
                }
            }

            //minute
            if (dateValue.Minute < 10)
                item.Add("0" + dateValue.Minute.ToString());
            else
                item.Add(dateValue.Minute.ToString());

            //format
            if (dateValue.Hour < 12)
            {
                item.Add("AM");
            }
            else
            {
                item.Add("PM");
            }

            return item;
        }

        public static string DateToString(this DateTime input, string format = "")
        {
            var retValue = string.Empty;
            format = (string.IsNullOrWhiteSpace(format) ? FormHelper.DateFormat : format);

            if (input > Constants.NullDate)
            {
                retValue = input.ToString(format);
            }

            return retValue;
        }
    }
}