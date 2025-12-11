using EatWork.Mobile.Models.Payslip;
using EatWork.Mobile.Utils;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Payslip
{
    public class PayslipDetailHolder : ExtendedBindableObject
    {
        public PayslipDetailHolder()
        {
            Model = new PayslipDetailModel();
            Earnings = new ObservableCollection<PaysheetDetailDto>();
            Allowances = new ObservableCollection<PaysheetDetailDto>();
            Deductions = new ObservableCollection<PaysheetDetailDto>();
            Loans = new ObservableCollection<PaysheetDetailDto>();
            YTDS = new ObservableCollection<PaysheetDetailDto>();
            RunningBalances = new ObservableCollection<PaysheetDetailDto>();
        }

        private PayslipDetailModel model_;

        public PayslipDetailModel Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        private ObservableCollection<PaysheetDetailDto> earnings_;

        public ObservableCollection<PaysheetDetailDto> Earnings
        {
            get { return earnings_; }
            set { earnings_ = value; RaisePropertyChanged(() => Earnings); }
        }

        private ObservableCollection<PaysheetDetailDto> allowances_;

        public ObservableCollection<PaysheetDetailDto> Allowances
        {
            get { return allowances_; }
            set { allowances_ = value; RaisePropertyChanged(() => Allowances); }
        }

        private ObservableCollection<PaysheetDetailDto> deductions_;

        public ObservableCollection<PaysheetDetailDto> Deductions
        {
            get { return deductions_; }
            set { deductions_ = value; RaisePropertyChanged(() => Deductions); }
        }

        private ObservableCollection<PaysheetDetailDto> loans_;

        public ObservableCollection<PaysheetDetailDto> Loans
        {
            get { return loans_; }
            set { loans_ = value; RaisePropertyChanged(() => Loans); }
        }

        private ObservableCollection<PaysheetDetailDto> ytds_;

        public ObservableCollection<PaysheetDetailDto> YTDS
        {
            get { return ytds_; }
            set { ytds_ = value; RaisePropertyChanged(() => YTDS); }
        }

        private ObservableCollection<PaysheetDetailDto> runningBalances_;

        public ObservableCollection<PaysheetDetailDto> RunningBalances
        {
            get { return runningBalances_; }
            set { runningBalances_ = value; RaisePropertyChanged(() => RunningBalances); }
        }
    }
}