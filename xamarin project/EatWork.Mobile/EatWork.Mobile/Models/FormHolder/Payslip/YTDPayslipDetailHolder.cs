using EatWork.Mobile.Models.Payslip;
using EatWork.Mobile.Utils;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Payslip
{
    public class YTDPayslipDetailHolder : ExtendedBindableObject
    {
        public YTDPayslipDetailHolder()
        {
            GrossEarnings = new ObservableCollection<PaysheetDetailDto>();
            Earnings = new ObservableCollection<PaysheetDetailDto>();
            YTDDetails = new ObservableCollection<PaysheetDetailDto>();
            OvertimeDetails = new ObservableCollection<PaysheetDetailDto>();
            LoanDetails = new ObservableCollection<PaysheetDetailDto>();
            LeaveDetails = new ObservableCollection<LeaveBalanceDetailDto>();
            DeductionBalances = new ObservableCollection<DeductionBalanceDetailDto>();
            TaxableEarnings = new ObservableCollection<PaysheetDetailDto>();
            NonTaxableEarnings = new ObservableCollection<PaysheetDetailDto>();
            OtherDedDetails = new ObservableCollection<PaysheetDetailDto>();
            DeductionDetails = new ObservableCollection<PaysheetDetailDto>();
            LoanBalances = new ObservableCollection<DeductionBalanceDetailDto>();
            Model = new PayslipDetailModel();
        }

        private PayslipDetailModel model_;

        public PayslipDetailModel Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        private ObservableCollection<PaysheetDetailDto> grossEarnings_;

        public ObservableCollection<PaysheetDetailDto> GrossEarnings
        {
            get { return grossEarnings_; }
            set { grossEarnings_ = value; RaisePropertyChanged(() => GrossEarnings); }
        }

        private ObservableCollection<PaysheetDetailDto> earnings_;

        public ObservableCollection<PaysheetDetailDto> Earnings
        {
            get { return earnings_; }
            set { earnings_ = value; RaisePropertyChanged(() => Earnings); }
        }

        private ObservableCollection<PaysheetDetailDto> ytdDetails_;

        public ObservableCollection<PaysheetDetailDto> YTDDetails
        {
            get { return ytdDetails_; }
            set { ytdDetails_ = value; RaisePropertyChanged(() => YTDDetails); }
        }

        private ObservableCollection<PaysheetDetailDto> overtimeDetails_;

        public ObservableCollection<PaysheetDetailDto> OvertimeDetails
        {
            get { return overtimeDetails_; }
            set { overtimeDetails_ = value; RaisePropertyChanged(() => OvertimeDetails); }
        }

        private string totalOtHours_;

        public string TotalOTHours
        {
            get { return totalOtHours_; }
            set { totalOtHours_ = value; RaisePropertyChanged(() => TotalOTHours); }
        }

        private string totalOtPay_;

        public string TotalOTPay
        {
            get { return totalOtPay_; }
            set { totalOtPay_ = value; RaisePropertyChanged(() => TotalOTPay); }
        }

        private ObservableCollection<PaysheetDetailDto> loanDetails_;

        public ObservableCollection<PaysheetDetailDto> LoanDetails
        {
            get { return loanDetails_; }
            set { loanDetails_ = value; RaisePropertyChanged(() => LoanDetails); }
        }

        private ObservableCollection<LeaveBalanceDetailDto> leaveDetails_;

        public ObservableCollection<LeaveBalanceDetailDto> LeaveDetails
        {
            get { return leaveDetails_; }
            set { leaveDetails_ = value; RaisePropertyChanged(() => LeaveDetails); }
        }

        private ObservableCollection<DeductionBalanceDetailDto> deductionBalances_;

        public ObservableCollection<DeductionBalanceDetailDto> DeductionBalances
        {
            get { return deductionBalances_; }
            set { deductionBalances_ = value; RaisePropertyChanged(() => DeductionBalances); }
        }

        private ObservableCollection<DeductionBalanceDetailDto> loanBalances_;

        public ObservableCollection<DeductionBalanceDetailDto> LoanBalances
        {
            get { return loanBalances_; }
            set { loanBalances_ = value; RaisePropertyChanged(() => LoanBalances); }
        }

        private ObservableCollection<PaysheetDetailDto> taxableEarnings_;

        public ObservableCollection<PaysheetDetailDto> TaxableEarnings
        {
            get { return taxableEarnings_; }
            set { taxableEarnings_ = value; RaisePropertyChanged(() => TaxableEarnings); }
        }

        private ObservableCollection<PaysheetDetailDto> nonTaxableEarnings_;

        public ObservableCollection<PaysheetDetailDto> NonTaxableEarnings
        {
            get { return nonTaxableEarnings_; }
            set { nonTaxableEarnings_ = value; RaisePropertyChanged(() => NonTaxableEarnings); }
        }

        private ObservableCollection<PaysheetDetailDto> deductionDetails_;

        public ObservableCollection<PaysheetDetailDto> DeductionDetails
        {
            get { return deductionDetails_; }
            set { deductionDetails_ = value; RaisePropertyChanged(() => DeductionDetails); }
        }

        private ObservableCollection<PaysheetDetailDto> otherDedDetails_;

        public ObservableCollection<PaysheetDetailDto> OtherDedDetails
        {
            get { return otherDedDetails_; }
            set { otherDedDetails_ = value; RaisePropertyChanged(() => OtherDedDetails); }
        }

        private string totalEarnedHoursDisplay_;

        public string TotalEarnedHoursDisplay
        {
            get { return totalEarnedHoursDisplay_; }
            set { totalEarnedHoursDisplay_ = value; RaisePropertyChanged(() => TotalEarnedHoursDisplay); }
        }

        private string totalUsedHoursDisplay_;

        public string TotalUsedHoursDisplay
        {
            get { return totalUsedHoursDisplay_; }
            set { totalUsedHoursDisplay_ = value; RaisePropertyChanged(() => TotalUsedHoursDisplay); }
        }

        private string totalCurrentBalanceDisplay_;

        public string TotalCurrentBalanceDisplay
        {
            get { return totalCurrentBalanceDisplay_; }
            set { totalCurrentBalanceDisplay_ = value; RaisePropertyChanged(() => TotalCurrentBalanceDisplay); }
        }

        private string totalOtherDeductionsDisplay_;

        public string TotalOtherDeductionDisplay
        {
            get { return totalOtherDeductionsDisplay_; }
            set { totalOtherDeductionsDisplay_ = value; RaisePropertyChanged(() => TotalOtherDeductionDisplay); }
        }

        private string totalLoanDisplay_;

        public string TotalLoanDisplay
        {
            get { return totalLoanDisplay_; }
            set { totalLoanDisplay_ = value; RaisePropertyChanged(() => TotalLoanDisplay); }
        }

        private string totalDeductionDisplay_;

        public string TotalDeductionDisplay
        {
            get { return totalDeductionDisplay_; }
            set { totalDeductionDisplay_ = value; RaisePropertyChanged(() => TotalDeductionDisplay); }
        }
    }
}