using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.Expense;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EatWork.Mobile.Models.FormHolder.Expenses
{
    public class MyExpensesListHolder : ExtendedBindableObject
    {
        public MyExpensesListHolder()
        {
            MyExpenses = new ObservableCollection<MyExpensesListDto>();
            IsMultipleMode = false;
            SelectedTaskCount = 0;
            SelectedDetail = new ObservableCollection<MyExpensesListDto>();
            SelectedTypes = new ObservableCollection<SelectableListModel>();
            ExpenseTypes = new ObservableCollection<SelectableListModel>();
            SelectAllText = "Select All";
        }

        private ObservableCollection<MyExpensesListDto> myExpenses_;

        public ObservableCollection<MyExpensesListDto> MyExpenses
        {
            get { return myExpenses_; }
            set { myExpenses_ = value; OnPropertyChanged(nameof(MyExpenses)); }
        }

        private bool _isMultipleMode;

        public bool IsMultipleMode
        {
            get { return _isMultipleMode; }
            set { _isMultipleMode = value; RaisePropertyChanged(() => IsMultipleMode); }
        }

        private int _selectedTaskCount;

        public int SelectedTaskCount
        {
            get { return _selectedTaskCount; }
            set { _selectedTaskCount = value; RaisePropertyChanged(() => SelectedTaskCount); }
        }

        private DateTime? startDate_;

        public DateTime? StartDate
        {
            get { return startDate_; }
            set { startDate_ = value; RaisePropertyChanged(() => StartDate); }
        }

        private DateTime? endDate_;

        public DateTime? EndDate
        {
            get { return endDate_; }
            set { endDate_ = value; RaisePropertyChanged(() => EndDate); }
        }

        private ObservableCollection<SelectableListModel> selectedTypes_;

        public ObservableCollection<SelectableListModel> SelectedTypes
        {
            get { return selectedTypes_; }
            set { selectedTypes_ = value; RaisePropertyChanged(() => SelectedTypes); }
        }

        private ObservableCollection<SelectableListModel> expenseTypes_;

        public ObservableCollection<SelectableListModel> ExpenseTypes
        {
            get { return expenseTypes_; }
            set { expenseTypes_ = value; RaisePropertyChanged(() => ExpenseTypes); }
        }

        private ObservableCollection<MyExpensesListDto> _selectedExpenseDetail;

        public ObservableCollection<MyExpensesListDto> SelectedDetail
        {
            get
            {
                return this._selectedExpenseDetail;
            }

            set
            {
                if (this._selectedExpenseDetail == value)
                {
                    return;
                }

                this._selectedExpenseDetail = value;
                RaisePropertyChanged(() => SelectedDetail);
            }
        }

        private string selectAllText_;

        public string SelectAllText
        {
            get { return selectAllText_; }
            set { selectAllText_ = value; RaisePropertyChanged(() => SelectAllText); }
        }
    }

    public class MyExpensesListDto : MyExpensesList, INotifyPropertyChanged
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked")); }
        }

        private bool _hasAttachment;

        public bool HasAttachment
        {
            get { return _hasAttachment; }
            set { _hasAttachment = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasAttachment")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}