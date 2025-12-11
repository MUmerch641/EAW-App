using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.DataObjects;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class OvertimeRequestHolder : RequestHolder
    {
        public OvertimeRequestHolder()
        {
            FileData = new FileData();
            IsPreshift = 0;
            IsOffSetting = false;
            OffsetExpirationDateToggle = true;
            //StartTime = DateTime.Now.TimeOfDay;
            //EndTime = DateTime.Now.TimeOfDay;
            WSStartTime = DateTime.Now.TimeOfDay;
            WSEndTime = DateTime.Now.TimeOfDay;
            ErrorDate = false;
            ErrorDateMessage = string.Empty;
            TimeInLimit = 0;
            TimeOutLimit = 0;
            AllowEarlyTimeIn = false;
            EnablePreshiftOT = true;
            EarlyTimeInLimit = 0;
            EarlyTimeInIsOvertime = false;
            MinimumOT = 0;
            IncludePreshiftInMinOT = false;
            IsOffsetEnabled = false;
            ShowWSField = false;
            OffSetExpirationDate = Constants.NullDate;
            StartTimeString = string.Empty;
            EndTimeString = string.Empty;
            MinimumOTHoursToggle = false;
            OverrideMinimumOT = false;
            ErrorStartTime = false;
            ErrorEndTime = false;
        }

        private ObservableCollection<ComboBoxObject> overtimeReason_;

        public ObservableCollection<ComboBoxObject> OvertimeReason
        {
            get { return overtimeReason_; }
            set { overtimeReason_ = value; RaisePropertyChanged(() => OvertimeReason); }
        }

        private ObservableCollection<string> preshiftOption_;

        public ObservableCollection<string> PreshiftOption
        {
            get { return preshiftOption_; }
            set { preshiftOption_ = value; RaisePropertyChanged(() => PreshiftOption); }
        }

        private ComboBoxObject overtimeReasonSelectedItem_;

        public ComboBoxObject OvertimeReasonSelectedItem
        {
            get { return overtimeReasonSelectedItem_; }
            set { overtimeReasonSelectedItem_ = value; RaisePropertyChanged(() => OvertimeReasonSelectedItem); }
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

        private TimeSpan? wsStartTime_;

        public TimeSpan? WSStartTime
        {
            get { return wsStartTime_; }
            set { wsStartTime_ = value; RaisePropertyChanged(() => StartTime); }
        }

        private TimeSpan? wsEndTime_;

        public TimeSpan? WSEndTime
        {
            get { return wsEndTime_; }
            set { wsEndTime_ = value; RaisePropertyChanged(() => EndTime); }
        }

        private bool showWSField_;

        public bool ShowWSField
        {
            get { return showWSField_; }
            set { showWSField_ = value; RaisePropertyChanged(() => ShowWSField); }
        }

        private ObservableCollection<object> startDateTime_;

        public ObservableCollection<object> StartDateTime
        {
            get { return startDateTime_; }
            set { startDateTime_ = value; RaisePropertyChanged(() => StartDateTime); }
        }

        private ObservableCollection<object> endDateTime_;

        public ObservableCollection<object> EndDateTime
        {
            get { return endDateTime_; }
            set { endDateTime_ = value; RaisePropertyChanged(() => EndDateTime); }
        }

        private short isPreshift_;

        public short IsPreshift
        {
            get { return isPreshift_; }
            set { isPreshift_ = value; RaisePropertyChanged(() => IsPreshift); }
        }

        private bool isOffSetting_;

        public bool IsOffSetting
        {
            get { return isOffSetting_; }
            set { isOffSetting_ = value; RaisePropertyChanged(() => IsOffSetting); }
        }

        private bool offsetExpirationDateToggle_;

        public bool OffsetExpirationDateToggle
        {
            get { return offsetExpirationDateToggle_; }
            set { offsetExpirationDateToggle_ = value; RaisePropertyChanged(() => OffsetExpirationDateToggle); }
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

        private decimal timeInLimit_;

        public decimal TimeInLimit
        {
            get { return timeInLimit_; }
            set { timeInLimit_ = value; RaisePropertyChanged(() => TimeInLimit); }
        }

        private decimal timeOutLimit_;

        public decimal TimeOutLimit
        {
            get { return timeOutLimit_; }
            set { timeOutLimit_ = value; RaisePropertyChanged(() => TimeOutLimit); }
        }

        private bool allowEarlyTimeIn_;

        public bool AllowEarlyTimeIn
        {
            get { return allowEarlyTimeIn_; }
            set { allowEarlyTimeIn_ = value; RaisePropertyChanged(() => AllowEarlyTimeIn); }
        }

        private bool enablePreshiftOT_;

        public bool EnablePreshiftOT
        {
            get { return enablePreshiftOT_; }
            set { enablePreshiftOT_ = value; RaisePropertyChanged(() => EnablePreshiftOT); }
        }

        private bool earlyTimeInIsOvertime_;

        public bool EarlyTimeInIsOvertime
        {
            get { return earlyTimeInIsOvertime_; }
            set { earlyTimeInIsOvertime_ = value; RaisePropertyChanged(() => EarlyTimeInIsOvertime); }
        }

        private decimal earlyTimeInLimit_;

        public decimal EarlyTimeInLimit
        {
            get { return earlyTimeInLimit_; }
            set { earlyTimeInLimit_ = value; RaisePropertyChanged(() => EarlyTimeInLimit); }
        }

        private decimal minimumOT_;

        public decimal MinimumOT
        {
            get { return minimumOT_; }
            set { minimumOT_ = value; RaisePropertyChanged(() => MinimumOT); }
        }

        private bool includePreshiftInMinOT_;

        public bool IncludePreshiftInMinOT
        {
            get { return includePreshiftInMinOT_; }
            set { includePreshiftInMinOT_ = value; RaisePropertyChanged(() => IncludePreshiftInMinOT); }
        }

        private bool isOffsetEnabled_;

        public bool IsOffsetEnabled
        {
            get { return isOffsetEnabled_; }
            set { isOffsetEnabled_ = value; RaisePropertyChanged(() => IsOffsetEnabled); }
        }

        private DateTime offSetExpirationDate_;

        public DateTime OffSetExpirationDate
        {
            get { return offSetExpirationDate_; }
            set { offSetExpirationDate_ = value; RaisePropertyChanged(() => OffSetExpirationDate); }
        }

        private bool minimumOTHoursToggle_;

        public bool MinimumOTHoursToggle
        {
            get { return minimumOTHoursToggle_; }
            set { minimumOTHoursToggle_ = value; RaisePropertyChanged(() => MinimumOTHoursToggle); }
        }

        private bool overrideMinimumOT_;

        public bool OverrideMinimumOT
        {
            get { return overrideMinimumOT_; }
            set { overrideMinimumOT_ = value; RaisePropertyChanged(() => OverrideMinimumOT); }
        }

        private OvertimeModel overtimeModel_;

        public OvertimeModel OvertimeModel
        {
            get { return overtimeModel_; }
            set { overtimeModel_ = value; RaisePropertyChanged(() => OvertimeModel); }
        }

        #region validators

        private bool errorOTHours_;

        public bool ErrorOTHours
        {
            get { return errorOTHours_; }
            set { errorOTHours_ = value; RaisePropertyChanged(() => ErrorOTHours); }
        }

        private bool errorReason_;

        public bool ErrorReason
        {
            get { return errorReason_; }
            set { errorReason_ = value; RaisePropertyChanged(() => ErrorReason); }
        }

        private bool errorDate_;

        public bool ErrorDate
        {
            get { return errorDate_; }
            set { errorDate_ = value; RaisePropertyChanged(() => ErrorDate); }
        }

        private string errorDateMessage_;

        public string ErrorDateMessage
        {
            get { return errorDateMessage_; }
            set { errorDateMessage_ = value; RaisePropertyChanged(() => ErrorDateMessage); }
        }

        private bool errorStartTime_;

        public bool ErrorStartTime
        {
            get { return errorStartTime_; }
            set { errorStartTime_ = value; RaisePropertyChanged(() => ErrorStartTime); }
        }

        private bool errorEndTime_;

        public bool ErrorEndTime
        {
            get { return errorEndTime_; }
            set { errorEndTime_ = value; RaisePropertyChanged(() => ErrorEndTime); }
        }

        #endregion validators
    }
}