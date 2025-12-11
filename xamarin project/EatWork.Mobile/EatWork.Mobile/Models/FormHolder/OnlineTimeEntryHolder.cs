using EatWork.Mobile.Contants;
using EatWork.Mobile.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Timers;

namespace EatWork.Mobile.Models.FormHolder
{
    public class OnlineTimeEntryHolder : BaseViewModel
    {
        public OnlineTimeEntryHolder()
        {
            TimeInColor = ClockworkColor.Success;
            BreakInColor = ClockworkColor.Pink;
            BreakOutColor = ClockworkColor.Success;
            TimeOutColor = ClockworkColor.Pink;
            ResponseMesage = string.Empty;
            EnableButtons = true;
            IsSuccess = false;
            ShowMessage = false;
            Latitude = 0;
            Longitude = 0;
            IpAddress = string.Empty;
            PublicIpAddress = string.Empty;
            HasSetup = true;
            LeaveWarningOnly = false;
            UserError = string.Empty;
            ClockworkConfiguration = new ClockworkConfigurationModel();
            UserErrorList = new ObservableCollection<string>();
            ShowForm = true;
        }

        private DateTime timeClock_;

        public DateTime TimeClock
        {
            get { return timeClock_; }
            set { timeClock_ = value; RaisePropertyChanged(() => TimeClock); }
        }

        private string employeeNumber_;

        public string EmployeeNumber
        {
            get { return employeeNumber_; }
            set { employeeNumber_ = value; RaisePropertyChanged(() => EmployeeNumber); }
        }

        private string accessCode_;

        public string AccessCode
        {
            get { return accessCode_; }
            set { accessCode_ = value; RaisePropertyChanged(() => AccessCode); }
        }

        private Timer timer_;

        public Timer Timer
        {
            get { return timer_; }
            set { timer_ = value; RaisePropertyChanged(() => Timer); }
        }

        private bool showBreakInOut_;

        public bool ShowBreakInOut
        {
            get { return showBreakInOut_; }
            set { showBreakInOut_ = value; RaisePropertyChanged(() => ShowBreakInOut); }
        }

        private bool enableButtons_;

        public bool EnableButtons
        {
            get { return enableButtons_; }
            set { enableButtons_ = value; RaisePropertyChanged(() => EnableButtons); }
        }

        private string timeInColor_;

        public string TimeInColor
        {
            get { return timeInColor_; }
            set { timeInColor_ = value; RaisePropertyChanged(() => TimeInColor); }
        }

        private string timeOutColor_;

        public string TimeOutColor
        {
            get { return timeOutColor_; }
            set { timeOutColor_ = value; RaisePropertyChanged(() => TimeOutColor); }
        }

        private string breakInColor_;

        public string BreakInColor
        {
            get { return breakInColor_; }
            set { breakInColor_ = value; RaisePropertyChanged(() => BreakInColor); }
        }

        private string breakOutColor_;

        public string BreakOutColor
        {
            get { return breakOutColor_; }
            set { breakOutColor_ = value; RaisePropertyChanged(() => BreakOutColor); }
        }

        private double longitude_;

        public double Longitude
        {
            get { return longitude_; }
            set { longitude_ = value; RaisePropertyChanged(() => Longitude); }
        }

        private double latitude_;

        public double Latitude
        {
            get { return latitude_; }
            set { latitude_ = value; RaisePropertyChanged(() => Latitude); }
        }

        private bool locationRequired_;

        public bool LocationRequired
        {
            get { return locationRequired_; }
            set { locationRequired_ = value; RaisePropertyChanged(() => LocationRequired); }
        }

        private string responseMessage_;

        public string ResponseMesage
        {
            get { return responseMessage_; }
            set { responseMessage_ = value; RaisePropertyChanged(() => ResponseMesage); }
        }

        private bool isSuccess_;

        public bool IsSuccess
        {
            get { return isSuccess_; }
            set { isSuccess_ = value; RaisePropertyChanged(() => IsSuccess); }
        }

        private bool showMessage_;

        public bool ShowMessage
        {
            get { return showMessage_; }
            set { showMessage_ = value; RaisePropertyChanged(() => ShowMessage); }
        }

        private bool hasSetup_;

        public bool HasSetup
        {
            get { return hasSetup_; }
            set { hasSetup_ = value; RaisePropertyChanged(() => HasSetup); }
        }

        private string ipAddress_;

        public string IpAddress
        {
            get { return ipAddress_; }
            set { ipAddress_ = value; RaisePropertyChanged(() => IpAddress); }
        }

        private string publicIpAddress_;

        public string PublicIpAddress
        {
            get { return publicIpAddress_; }
            set { publicIpAddress_ = value; RaisePropertyChanged(() => PublicIpAddress); }
        }

        private bool allowImageCapture_;

        public bool AllowImageCapture
        {
            get { return allowImageCapture_; }
            set { allowImageCapture_ = value; RaisePropertyChanged(() => AllowImageCapture); }
        }

        private bool allowLocationCapture_;

        public bool AllowLocationCapture
        {
            get { return allowLocationCapture_; }
            set { allowLocationCapture_ = value; RaisePropertyChanged(() => AllowLocationCapture); }
        }

        private string userError_;

        public string UserError
        {
            get { return userError_; }
            set { userError_ = value; RaisePropertyChanged(() => UserError); }
        }

        private bool leaveWarningOnly_;

        public bool LeaveWarningOnly
        {
            get { return leaveWarningOnly_; }
            set { leaveWarningOnly_ = value; RaisePropertyChanged(() => LeaveWarningOnly); }
        }

        private bool showForm_;

        public bool ShowForm
        {
            get { return showForm_; }
            set { showForm_ = value; RaisePropertyChanged(() => ShowForm); }
        }

        private ObservableCollection<string> userErrorList_;

        public ObservableCollection<string> UserErrorList
        {
            get { return userErrorList_; }
            set { userErrorList_ = value; RaisePropertyChanged(() => UserErrorList); }
        }

        private ClockworkConfigurationModel clockworkConfiguration_;

        public ClockworkConfigurationModel ClockworkConfiguration
        {
            get { return clockworkConfiguration_; }
            set { clockworkConfiguration_ = value; RaisePropertyChanged(() => ClockworkConfiguration); }
        }

        private TimeEntryLogModel model;

        public TimeEntryLogModel TimeEntryLogModel
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(() => TimeEntryLogModel); }
        }

        #region validators

        private bool errorEmployeeNumber_;

        public bool ErrorEmployeeNumber
        {
            get { return errorEmployeeNumber_; }
            set { errorEmployeeNumber_ = value; RaisePropertyChanged(() => ErrorEmployeeNumber); }
        }

        private bool errorAccessCode_;

        public bool ErrorAccessCode
        {
            get { return errorAccessCode_; }
            set { errorAccessCode_ = value; RaisePropertyChanged(() => ErrorAccessCode); }
        }

        #endregion validators
    }
}