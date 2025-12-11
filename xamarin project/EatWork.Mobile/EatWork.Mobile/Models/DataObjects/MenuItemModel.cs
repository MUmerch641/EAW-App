using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EatWork.Mobile.Models.DataObjects
{
    public class MenuItemModel : INotifyPropertyChanged
    {
        public MenuItemModel()
        {
            TrailingIcon = "";
            IsVisible = false;
            IsDefault = false;
        }

        public MenuItemType Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Command { get; set; }
        public Type TargetType { get; set; }
        public bool IsSeparatorVisible { get; internal set; }
        public int GroupId { get; internal set; }
        public bool IsDefault { get; internal set; }
        public ObservableCollection<MenuItemModel> SubItems { get; set; }
        public MenuGroup MenuGroupId { get; set; }

        private string _trailingIcon;

        public string TrailingIcon
        {
            get { return _trailingIcon; }
            set { _trailingIcon = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrailingIcon")); }
        }

        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisible")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum MenuItemType
    {
        MyRequest,
        MyApproval,
        MySchedule,
        MyTimeLog,
        Attendance,
        CashAdvanceRequest,
        Payroll,
        ExpenseReport,
        MyProfile,
        Settings,
        Notifications,
        LogOut,
        MyPayslip,
        MyExpenses,
        MyExpenseReport,
        Expenses,
        Home,
        IndividualObjectives,
        SuggestionCorner,
        PerformanceEvaluation,
        EmployeeRelations,
        MyAttendance,
        Leave,
        Overtime,
        OfficialBusiness,
        Undertime,
        TimeEntryLog,
        TimeOff,
        ChangeWorkSchedule,
        ChangeRestday,
        SpecialWorkSchedule,
        Loan,
        Document,
        TravelRequest,
        Clockwork,
        YTDPayslipTemplate,
        PulseSurveyForm,
        ThemeConfigForm
    }

    public enum MenuGroup
    {
        Home,
        MyRequest,
        MyApproval,
        Attendance,
        Payroll,
        Expenses,
        Performance,
        EmployeeRelations,
        Settings,
        SingOut,
    }
}