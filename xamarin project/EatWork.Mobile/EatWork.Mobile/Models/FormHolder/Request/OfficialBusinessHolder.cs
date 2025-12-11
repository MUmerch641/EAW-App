using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.DataObjects;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class OfficialBusinessHolder : RequestHolder
    {
        public OfficialBusinessHolder()
        {
            ErrorOBReason = false;
            ErrorOBStartDate = false;
            ErrorRemarks = false;
            ErrorNoOfHours = false;
            ErrorOBApplyTo = false;
            OBStartDate = DateTime.Now;
            OBEndDate = DateTime.Now;
            //StartTime = DateTime.Now.TimeOfDay;
            //EndTime = DateTime.Now.TimeOfDay;
            SkipRestdays = false;
            SkipHolidays = false;
            OverrideSchedule = false;
            StartTimeString = Constants.NullDate.ToString(Constants.TimeFormatHHMMTT);
            EndTimeString = Constants.NullDate.ToString(Constants.TimeFormatHHMMTT);
        }

        private ObservableCollection<ComboBoxObject> obApplyTo_;

        public ObservableCollection<ComboBoxObject> OBApplyTo
        {
            get { return obApplyTo_; }
            set { obApplyTo_ = value; RaisePropertyChanged(() => OBApplyTo); }
        }

        private ComboBoxObject obApplyToSelectedItem_;

        public ComboBoxObject OBApplyToSelectedItem
        {
            get { return obApplyToSelectedItem_; }
            set { obApplyToSelectedItem_ = value; RaisePropertyChanged(() => OBApplyToSelectedItem); }
        }

        private ObservableCollection<ComboBoxObject> obReason_;

        public ObservableCollection<ComboBoxObject> OBReason
        {
            get { return obReason_; }
            set { obReason_ = value; RaisePropertyChanged(() => OBReason); }
        }

        private ComboBoxObject obReasonSelectedItem_;

        public ComboBoxObject OBReasonSelectedItem
        {
            get { return obReasonSelectedItem_; }
            set { obReasonSelectedItem_ = value; RaisePropertyChanged(() => OBReasonSelectedItem); }
        }

        private bool startCheckboxEnabled_;

        public bool StartCheckboxEnabled
        {
            get { return startCheckboxEnabled_; }
            set { startCheckboxEnabled_ = value; RaisePropertyChanged(() => StartCheckboxEnabled); }
        }

        private bool endCheckboxEnabled_;

        public bool EndCheckboxEnabled
        {
            get { return endCheckboxEnabled_; }
            set { endCheckboxEnabled_ = value; RaisePropertyChanged(() => EndCheckboxEnabled); }
        }

        private DateTime? obStartDate_;

        public DateTime? OBStartDate
        {
            get { return obStartDate_; }
            set { obStartDate_ = value; RaisePropertyChanged(() => OBStartDate); }
        }

        private DateTime? obEndDate_;

        public DateTime? OBEndDate
        {
            get { return obEndDate_; }
            set { obEndDate_ = value; RaisePropertyChanged(() => OBEndDate); }
        }

        private TimeSpan? startTime_;

        public TimeSpan? StartTime
        {
            get { return startTime_; }
            set { startTime_ = value; RaisePropertyChanged(() => StartTime); }
        }

        private TimeSpan? endTime_;

        public TimeSpan? EndTime
        {
            get { return endTime_; }
            set { endTime_ = value; RaisePropertyChanged(() => EndTime); }
        }

        private string startTimeString_;

        public string StartTimeString
        {
            get { return startTimeString_; }
            set { startTimeString_ = value; RaisePropertyChanged(() => StartTimeString); }
        }

        private string endTimeString_;

        public string EndTimeString
        {
            get { return endTimeString_; }
            set { endTimeString_ = value; RaisePropertyChanged(() => EndTimeString); }
        }

        private OfficialBusinessModel model_;

        public OfficialBusinessModel OfficialBusinessModel
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => OfficialBusinessModel); }
        }

        private bool overrideSchedule_;

        public bool OverrideSchedule
        {
            get { return overrideSchedule_; }
            set { overrideSchedule_ = value; RaisePropertyChanged(() => OverrideSchedule); }
        }

        private bool skipHolidays_;

        public bool SkipHolidays
        {
            get { return skipHolidays_; }
            set { skipHolidays_ = value; RaisePropertyChanged(() => SkipHolidays); }
        }

        private bool skipRestdays_;

        public bool SkipRestdays
        {
            get { return skipRestdays_; }
            set { skipRestdays_ = value; RaisePropertyChanged(() => SkipRestdays); }
        }

        private bool includePreFilingDates_;

        public bool IncludePreFilingDates
        {
            get { return includePreFilingDates_; }
            set { includePreFilingDates_ = value; RaisePropertyChanged(() => IncludePreFilingDates); }
        }

        #region validators

        private bool errorOBStartDate_;

        public bool ErrorOBStartDate
        {
            get { return errorOBStartDate_; }
            set { errorOBStartDate_ = value; RaisePropertyChanged(() => ErrorOBStartDate); }
        }

        private bool noOfHours_;

        public bool NoOfHours
        {
            get { return noOfHours_; }
            set { noOfHours_ = value; RaisePropertyChanged(() => NoOfHours); }
        }

        private bool errorOBReason_;

        public bool ErrorOBReason
        {
            get { return errorOBReason_; }
            set { errorOBReason_ = value; RaisePropertyChanged(() => ErrorOBReason); }
        }

        private bool errorRemarks_;

        public bool ErrorRemarks
        {
            get { return errorRemarks_; }
            set { errorRemarks_ = value; RaisePropertyChanged(() => ErrorRemarks); }
        }

        private bool errorNoOfHours_;

        public bool ErrorNoOfHours
        {
            get { return errorNoOfHours_; }
            set { errorNoOfHours_ = value; RaisePropertyChanged(() => ErrorNoOfHours); }
        }

        private bool errorOBApplyTo_;

        public bool ErrorOBApplyTo
        {
            get { return errorOBApplyTo_; }
            set { errorOBApplyTo_ = value; RaisePropertyChanged(() => ErrorOBApplyTo); }
        }

        #endregion validators
    }
}