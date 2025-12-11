using EatWork.Mobile.Contants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Expenses
{
    public class ExpenseReportDetailHolder : RequestHolder
    {
        public ExpenseReportDetailHolder()
        {
            ReportNo = string.Empty;
            RecordId = 0;
            Date = DateTime.UtcNow.Date;
            DateDisplay = Date.ToString(FormHelper.DateFormat);
            Details = new ObservableCollection<ExpenseReportDetailModel>();
            Header = new ExpenseReportModel();
            AppExpenseDetailIds = new List<long>();
            TotalAmount = 0;
            ForSubmission = false;
        }

        private string reportNo_;

        public string ReportNo
        {
            get { return reportNo_; }
            set { reportNo_ = value; RaisePropertyChanged(() => ReportNo); }
        }

        private long recordId;

        public long RecordId
        {
            get { return recordId; }
            set { recordId = value; RaisePropertyChanged(() => RecordId); }
        }

        private DateTime date_;

        public DateTime Date
        {
            get { return date_; }
            set { date_ = value; RaisePropertyChanged(() => Date); }
        }

        private decimal totalAmount_;

        public decimal TotalAmount
        {
            get { return totalAmount_; }
            set { totalAmount_ = value; RaisePropertyChanged(() => TotalAmount); }
        }

        private string totalAmountDisplay_;

        public string TotalAmountDisplay
        {
            get { return totalAmountDisplay_; }
            set { totalAmountDisplay_ = value; RaisePropertyChanged(() => TotalAmountDisplay); }
        }

        public string DateDisplay { get; set; }

        private bool forSubmission_;

        public bool ForSubmission
        {
            get { return forSubmission_; }
            set { forSubmission_ = value; RaisePropertyChanged(() => TotalAmountDisplay); }
        }

        private ObservableCollection<ExpenseReportDetailModel> details_;

        public ObservableCollection<ExpenseReportDetailModel> Details
        {
            get { return details_; }
            set { details_ = value; RaisePropertyChanged(() => Details); }
        }

        private ExpenseReportModel header_;

        public ExpenseReportModel Header
        {
            get { return header_; }
            set { header_ = value; RaisePropertyChanged(() => Details); }
        }

        private List<long> appExpenseDetailIds_;

        public List<long> AppExpenseDetailIds
        {
            get { return appExpenseDetailIds_; }
            set { appExpenseDetailIds_ = value; RaisePropertyChanged(() => AppExpenseDetailIds); }
        }
    }
}