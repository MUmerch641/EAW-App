using EatWork.Mobile.Contants;
using Syncfusion.SfPicker.XForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Xamarin.Forms;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace EatWork.Mobile.Utils
{
    /// <summary>
    /// https://help.syncfusion.com/xamarin/picker/datetimepicker
    /// </summary>
    public class DateTimePicker : SfPicker
    {
        #region Public Properties

        // Months API is used to modify the Day collection as per change in Month

        internal Dictionary<string, string> Months { get; set; }

        /// <summary>

        /// Date is the actual DataSource for SfPicker control which will holds the collection of Day ,Month and Year

        /// </summary>

        /// <value>The date.</value>

        public ObservableCollection<object> Date { get; set; }

        //Day is the collection of day numbers

        internal ObservableCollection<object> Day { get; set; }

        //Month is the collection of Month Names

        internal ObservableCollection<object> Month { get; set; }

        //Year is the collection of Years from 1990 to 2042

        internal ObservableCollection<object> Year { get; set; }

        //Hour is the collection of Hours in Railway time format

        internal ObservableCollection<object> Hour { get; set; }

        //Minute is the collection of Minutes from 00 to 59

        internal ObservableCollection<object> Minute { get; set; }

        public ObservableCollection<object> Format;

        /// <summary>

        /// Headers API is holds the column name for every column in date picker

        /// </summary>

        /// <value>The Headers.</value>

        public ObservableCollection<string> Headers { get; set; }

        #endregion Public Properties

        public DateTimePicker()
        {
            Headers = new ObservableCollection<string>();

            Months = new Dictionary<string, string>();

            Date = new ObservableCollection<object>();

            Day = new ObservableCollection<object>();

            Month = new ObservableCollection<object>();

            Year = new ObservableCollection<object>();

            Hour = new ObservableCollection<object>();

            Minute = new ObservableCollection<object>();

            Format = new ObservableCollection<object>();

            PopulateDateCollection();

            this.ItemsSource = Date;

            //hook selection changed event
            this.SelectionChanged += CustomDatePicker_SelectionChanged;

            Headers.Add("Month");
            Headers.Add("Day");
            Headers.Add("Year");
            Headers.Add("Hour");
            Headers.Add("Minute");
            Headers.Add("");

            //SfPicker header text

            HeaderText = "Date Time Picker";

            this.ColumnHeaderText = Headers;

            //Enable Footer

            ShowFooter = true;

            //Enable SfPicker Header

            ShowHeader = true;

            //Enable Column Header of SfPicker

            ShowColumnHeader = true;
        }

        private void PopulateDateCollection()
        {
            //populate months

            for (int i = 1; i < 13; i++)
            {
                if (!Months.ContainsKey(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3)))

                    Months.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));

                Month.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3));
            }

            //populate year
            for (int i = 1990; i < 2050; i++)
            {
                Year.Add(i.ToString());
            }

            //populate Days
            for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                if (i < 10)

                {
                    Day.Add("0" + i);
                }
                else

                    Day.Add(i.ToString());
            }

            //populate Hours
            for (int i = 1; i <= 12; i++)
            {
                if (i < 10)

                {
                    Hour.Add("0" + i.ToString());
                }
                else

                    Hour.Add(i.ToString());
            }

            //populate Minutes
            for (int j = 0; j < 60; j++)
            {
                if (j < 10)

                {
                    Minute.Add("0" + j);
                }
                else

                    Minute.Add(j.ToString());
            }

            Format.Add("AM");

            Format.Add("PM");

            Date.Add(Month);
            Date.Add(Day);
            Date.Add(Year);
            Date.Add(Hour);
            Date.Add(Minute);
            Date.Add(Format);
        }

        private void CustomDatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDays(Date, e);
        }

        public void UpdateDays(ObservableCollection<object> Date, SelectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Date.Count == 5)
                {
                    bool flag = false;
                    if (e.OldValue != null && e.NewValue != null && (e.OldValue is ObservableCollection<object>) && (e.OldValue as ObservableCollection<object>).Count > 0)
                    {
                        if (!object.Equals((e.OldValue as IList)[1], (e.NewValue as IList)[1]))
                        {
                            flag = true;
                        }

                        if (!object.Equals((e.OldValue as IList)[0], (e.NewValue as IList)[0]))
                        {
                            flag = true;
                        }
                    }

                    if (flag)
                    {
                        ObservableCollection<object> days = new ObservableCollection<object>();
                        int month = DateTime.ParseExact(Months[(e.NewValue as IList)[1].ToString()], "MMMM", CultureInfo.InvariantCulture).Month;
                        int year = int.Parse((e.NewValue as IList)[0].ToString());
                        for (int j = 1; j <= DateTime.DaysInMonth(year, month); j++)
                        {
                            if (j < 10)
                            {
                                days.Add("0" + j);
                            }
                            else
                                days.Add(j.ToString());
                        }

                        ObservableCollection<object> PreviousValue = new ObservableCollection<object>();

                        foreach (var item in e.NewValue as IList)
                        {
                            PreviousValue.Add(item);
                        }

                        if (days.Count > 0)
                        {
                            Date.RemoveAt(2);
                            Date.Insert(2, days);
                        }

                        if ((Date[2] as IList).Contains(PreviousValue[2]))
                        {
                            this.SelectedItem = PreviousValue;
                        }
                        else
                        {
                            PreviousValue[2] = (Date[2] as IList)[(Date[2] as IList).Count - 1];
                            this.SelectedItem = PreviousValue;
                        }
                    }
                }
            });
        }
    }

    /// <summary>
    /// https://forums.xamarin.com/discussion/20028/datepicker-possible-to-bind-to-nullable-date-value
    /// </summary>
    ///
    public class NullableDatePicker : DatePicker
    {
        public static readonly BindableProperty NullableDateProperty = BindableProperty.Create(
            "NullableDate", typeof(DateTime?), typeof(NullableDatePicker), null, BindingMode.TwoWay);

        public static readonly BindableProperty EmptyStateTextProperty = BindableProperty.Create(
            "EmptyStateText", typeof(string), typeof(NullableDatePicker), string.Empty, BindingMode.OneWay);

        public DateTime? NullableDate
        {
            get { return (DateTime?)GetValue(NullableDateProperty); }
            set
            {
                if (value != NullableDate)
                {
                    SetValue(NullableDateProperty, value);
                    UpdateDate();
                }
            }
        }

        public string EmptyStateText
        {
            get { return (string)GetValue(EmptyStateTextProperty); }
            set { SetValue(EmptyStateTextProperty, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            UpdateDate();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsFocusedProperty.PropertyName)
            {
                if (IsFocused)
                {
                    if (!NullableDate.HasValue || NullableDate.Value <= Constants.NullDate)
                    {
                        NullableDate = DateTime.UtcNow.Date;
                        Date = DateTime.UtcNow.Date;
                        //Date = (DateTime)DateProperty.DefaultValue;
                    }
                }
                else
                {
                    OnPropertyChanged(DateProperty.PropertyName);
                }
            }

            if (propertyName == DateProperty.PropertyName)
            {
                NullableDate = Date;
            }

            if (propertyName == DateProperty.PropertyName)
            {
                if (NullableDate.HasValue)
                {
                    Date = NullableDate.Value;
                }
            }
        }

        private void UpdateDate()
        {
            if (NullableDate.HasValue)
            {
                Date = NullableDate.Value;
            }
            else
            {
                Date = (DateTime)DateProperty.DefaultValue;
                /*
                if (Device.RuntimePlatform == Device.iOS)
                {
                    NullableTime = DateTime.Now.TimeOfDay;
                }
                */
            }
        }
    }

    public class NullableTimePicker : TimePicker
    {
        public static readonly BindableProperty NullableTimeProperty = BindableProperty.Create(
           "NullableTime", typeof(TimeSpan?), typeof(NullableTimePicker), null, BindingMode.TwoWay);

        public static readonly BindableProperty EmptyStateTextProperty = BindableProperty.Create(
            "EmptyStateText", typeof(string), typeof(NullableTimePicker), string.Empty, BindingMode.OneWay);

        public TimeSpan? NullableTime
        {
            get { return (TimeSpan?)GetValue(NullableTimeProperty); }
            set
            {
                if (value != NullableTime)
                {
                    SetValue(NullableTimeProperty, value);
                    UpdateDate();
                }
            }
        }

        public string EmptyStateText
        {
            get { return (string)GetValue(EmptyStateTextProperty); }
            set { SetValue(EmptyStateTextProperty, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            UpdateDate();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsFocusedProperty.PropertyName)
            {
                if (IsFocused)
                {
                    if (!NullableTime.HasValue)
                    {
                        Time = (TimeSpan)TimeProperty.DefaultValue;
                    }
                }
                else
                {
                    OnPropertyChanged(TimeProperty.PropertyName);
                }
            }

            if (propertyName == TimeProperty.PropertyName)
            {
                NullableTime = Time;
            }

            if (propertyName == NullableTimeProperty.PropertyName)
            {
                if (NullableTime.HasValue)
                {
                    Time = NullableTime.Value;
                }
            }
        }

        private void UpdateDate()
        {
            if (NullableTime.HasValue)
            {
                Time = NullableTime.Value;
            }
            else
            {
                Time = (TimeSpan)TimeProperty.DefaultValue;
                /*
                if (Device.RuntimePlatform == Device.iOS)
                {
                    NullableTime = DateTime.Now.TimeOfDay;
                }
                */
            }
        }
    }
}