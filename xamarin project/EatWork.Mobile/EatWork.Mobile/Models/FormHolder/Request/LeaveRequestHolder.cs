using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.Leave;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class LeaveRequestHolder : RequestHolder
    {
        public LeaveRequestHolder()
        {
            ErrorLeaveType = false;
            ErrorPartialType = false;
            ShowPartialOptions = false;
            IsPartialLeave = 0;
            ApplyToOption = 0;
            InclusiveStartDate = DateTime.Now.Date;
            InclusiveEndDate = DateTime.Now.Date;
            Planned = false;
            LeaveType = new ObservableCollection<SelectableListModel>();
            ApplyTo = new ObservableCollection<ComboBoxObject>();
            LeaveRequestReason = new ObservableCollection<ComboBoxObject>();
            ApplyToSelectedItem = new ComboBoxObject();
            ReasonSelectedItem = new ComboBoxObject();
            AutomateHalfDayLeave = false;
            IsGenerated = false;
            LeaveTypeDocument = new ObservableCollection<SelectableListModel>();
            SelectedLeaveTypeDocument = new SelectableListModel();
            LeaveTypeDocumentHolder = new ObservableCollection<SelectableListModel>();
            LeaveRequestDetailList = new ObservableCollection<LeaveRequestDetailModel>();
            LeaveTypeSelectedItem = new SelectableListModel();
            LeaveDocumentModel = new ObservableCollection<LeaveRequestDocumentModel>();
            SelectedLeaveDocumentModel = new LeaveRequestDocumentModel();
            LeaveRequestHeaderModel = new LeaveRequestHeaderModel();
            LeaveRequestEngineModel = new LeaveRequestEngineModel();
            LeaveRequestDetailListToDisplay = new ObservableCollection<LeaveRequestDetailModel>();
            ShowDocumentList = false;
            DisplayInDays = false;
            BalanceLabelDisplay = "hrs";
            RequestLabelDisplay = "No. of Hours  ";
            RemainingBalance = 0;
            IsMultipleLeave = false;
            ApplyToAutomated = new ObservableCollection<SfSegmentItem>();
            ConflictList = new List<string>();
        }

        private ObservableCollection<ComboBoxObject> applyTo_;

        public ObservableCollection<ComboBoxObject> ApplyTo
        {
            get { return applyTo_; }
            set { applyTo_ = value; RaisePropertyChanged(() => ApplyTo); }
        }

        private ObservableCollection<SfSegmentItem> applyToAutomated_;

        public ObservableCollection<SfSegmentItem> ApplyToAutomated
        {
            get { return applyToAutomated_; }
            set { applyToAutomated_ = value; RaisePropertyChanged(() => ApplyToAutomated); }
        }

        private ObservableCollection<SelectableListModel> leaveType_;

        public ObservableCollection<SelectableListModel> LeaveType
        {
            get { return leaveType_; }
            set { leaveType_ = value; RaisePropertyChanged(() => LeaveType); }
        }

        private ObservableCollection<ComboBoxObject> leaveRequestReason_;

        public ObservableCollection<ComboBoxObject> LeaveRequestReason
        {
            get { return leaveRequestReason_; }
            set { leaveRequestReason_ = value; RaisePropertyChanged(() => LeaveRequestReason); }
        }

        private ObservableCollection<string> partialDayOption_;

        public ObservableCollection<string> PartialDayOption
        {
            get { return partialDayOption_; }
            set { partialDayOption_ = value; RaisePropertyChanged(() => PartialDayOption); }
        }

        private SelectableListModel leaveTypeSelectedItem_;

        public SelectableListModel LeaveTypeSelectedItem
        {
            get { return leaveTypeSelectedItem_; }
            set { leaveTypeSelectedItem_ = value; RaisePropertyChanged(() => LeaveTypeSelectedItem); }
        }

        private ComboBoxObject applyToSelectedItem_;

        public ComboBoxObject ApplyToSelectedItem
        {
            get { return applyToSelectedItem_; }
            set { applyToSelectedItem_ = value; RaisePropertyChanged(() => ApplyToSelectedItem); }
        }

        private ComboBoxObject reasonSelectedItem_;

        public ComboBoxObject ReasonSelectedItem
        {
            get { return reasonSelectedItem_; }
            set { reasonSelectedItem_ = value; RaisePropertyChanged(() => ReasonSelectedItem); }
        }

        private bool showPartialItems_;

        public bool ShowPartialOptions
        {
            get { return showPartialItems_; }
            set { showPartialItems_ = value; RaisePropertyChanged(() => ShowPartialOptions); }
        }

        private short isPartialLeave_;

        public short IsPartialLeave
        {
            get { return isPartialLeave_; }
            set { isPartialLeave_ = value; RaisePropertyChanged(() => IsPartialLeave); }
        }

        private short applyToOption_;

        public short ApplyToOption
        {
            get { return applyToOption_; }
            set { applyToOption_ = value; RaisePropertyChanged(() => ApplyToOption); }
        }

        private DateTime startDate_;

        public DateTime InclusiveStartDate
        {
            get { return startDate_; }
            set { startDate_ = value; RaisePropertyChanged(() => InclusiveStartDate); }
        }

        private DateTime endDate_;

        public DateTime InclusiveEndDate
        {
            get { return endDate_; }
            set { endDate_ = value; RaisePropertyChanged(() => InclusiveEndDate); }
        }

        private bool planned_;

        public bool Planned
        {
            get { return planned_; }
            set { planned_ = value; RaisePropertyChanged(() => Planned); }
        }

        private bool automatedHalfDayLeave_;

        public bool AutomateHalfDayLeave
        {
            get { return automatedHalfDayLeave_; }
            set { automatedHalfDayLeave_ = value; RaisePropertyChanged(() => AutomateHalfDayLeave); }
        }

        private bool isGenerated_;

        public bool IsGenerated
        {
            get { return isGenerated_; }
            set { isGenerated_ = value; RaisePropertyChanged(() => IsGenerated); }
        }

        private bool showDocumentList_;

        public bool ShowDocumentList
        {
            get { return showDocumentList_; }
            set { showDocumentList_ = value; RaisePropertyChanged(() => ShowDocumentList); }
        }

        private bool displayInDays_;

        public bool DisplayInDays
        {
            get { return displayInDays_; }
            set { displayInDays_ = value; RaisePropertyChanged(() => DisplayInDays); }
        }

        private string balanceLabelDisplay_;

        public string BalanceLabelDisplay
        {
            get { return balanceLabelDisplay_; }
            set { balanceLabelDisplay_ = value; RaisePropertyChanged(() => BalanceLabelDisplay); }
        }

        private string requestLabelDisplay_;

        public string RequestLabelDisplay
        {
            get { return requestLabelDisplay_; }
            set { requestLabelDisplay_ = value; RaisePropertyChanged(() => RequestLabelDisplay); }
        }

        private decimal remainingBalance_;

        public decimal RemainingBalance
        {
            get { return remainingBalance_; }
            set { remainingBalance_ = value; RaisePropertyChanged(() => RemainingBalance); }
        }

        private bool isMultipleLeave_;

        public bool IsMultipleLeave
        {
            get { return isMultipleLeave_; }
            set { isMultipleLeave_ = value; RaisePropertyChanged(() => IsMultipleLeave); }
        }

        private ObservableCollection<SelectableListModel> leaveTypeDocument_;

        public ObservableCollection<SelectableListModel> LeaveTypeDocument
        {
            get { return leaveTypeDocument_; }
            set { leaveTypeDocument_ = value; RaisePropertyChanged(() => LeaveTypeDocument); }
        }

        private ObservableCollection<SelectableListModel> leaveTypeDocumentHolder_;

        public ObservableCollection<SelectableListModel> LeaveTypeDocumentHolder
        {
            get { return leaveTypeDocumentHolder_; }
            set { leaveTypeDocumentHolder_ = value; RaisePropertyChanged(() => LeaveTypeDocumentHolder); }
        }

        private SelectableListModel selectedLeaveTypeDocument_;

        public SelectableListModel SelectedLeaveTypeDocument
        {
            get { return selectedLeaveTypeDocument_; }
            set { selectedLeaveTypeDocument_ = value; RaisePropertyChanged(() => SelectedLeaveTypeDocument); }
        }

        private ObservableCollection<LeaveRequestDetailModel> leaveRequestDetailList_;

        public ObservableCollection<LeaveRequestDetailModel> LeaveRequestDetailList
        {
            get { return leaveRequestDetailList_; }
            set { leaveRequestDetailList_ = value; RaisePropertyChanged(() => LeaveRequestDetailList); }
        }

        private ObservableCollection<LeaveRequestDetailModel> leaveRequestDetailListDisplay_;

        public ObservableCollection<LeaveRequestDetailModel> LeaveRequestDetailListToDisplay
        {
            get { return leaveRequestDetailListDisplay_; }
            set { leaveRequestDetailListDisplay_ = value; RaisePropertyChanged(() => LeaveRequestDetailListToDisplay); }
        }

        private ObservableCollection<LeaveRequestDocumentModel> leaveDocumentModel_;

        public ObservableCollection<LeaveRequestDocumentModel> LeaveDocumentModel
        {
            get { return leaveDocumentModel_; }
            set { leaveDocumentModel_ = value; RaisePropertyChanged(() => LeaveDocumentModel); }
        }

        private LeaveRequestDocumentModel selectedLeaveDocumentModel_;

        public LeaveRequestDocumentModel SelectedLeaveDocumentModel
        {
            get { return selectedLeaveDocumentModel_; }
            set { selectedLeaveDocumentModel_ = value; RaisePropertyChanged(() => SelectedLeaveDocumentModel); }
        }

        private LeaveRequestHeaderModel leaveRequestHeaderModel_;

        public LeaveRequestHeaderModel LeaveRequestHeaderModel
        {
            get { return leaveRequestHeaderModel_; }
            set { leaveRequestHeaderModel_ = value; RaisePropertyChanged(() => LeaveRequestHeaderModel); }
        }

        private LeaveRequestEngineModel leaveRequestEngineModel_;

        public LeaveRequestEngineModel LeaveRequestEngineModel
        {
            get { return leaveRequestEngineModel_; }
            set { leaveRequestEngineModel_ = value; RaisePropertyChanged(() => LeaveRequestEngineModel); }
        }

        private LeaveRequestModel leaveRequestModel_;

        public LeaveRequestModel LeaveRequestModel
        {
            get { return leaveRequestModel_; }
            set { leaveRequestModel_ = value; RaisePropertyChanged(() => LeaveRequestModel); }
        }

        private bool hasWarning_;

        public bool HasWarning
        {
            get { return hasWarning_; }
            set { hasWarning_ = value; RaisePropertyChanged(() => HasWarning); }
        }

        private List<string> conflictList_;

        public List<string> ConflictList
        {
            get { return conflictList_; }
            set { conflictList_ = value; RaisePropertyChanged(() => ConflictList); }
        }

        private string conflictMessage_;

        public string ConflictMessage
        {
            get { return conflictMessage_; }
            set { conflictMessage_ = value; RaisePropertyChanged(() => ConflictList); }
        }

        #region validators

        private bool errorLeaveType_;

        public bool ErrorLeaveType
        {
            get { return errorLeaveType_; }
            set { errorLeaveType_ = value; RaisePropertyChanged(() => ErrorLeaveType); }
        }

        private bool errorPartialType_;

        public bool ErrorPartialType
        {
            get { return errorPartialType_; }
            set { errorPartialType_ = value; RaisePropertyChanged(() => ErrorPartialType); }
        }

        #endregion validators
    }
}