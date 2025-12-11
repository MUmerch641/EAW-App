using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Payslip;
using System.Threading.Tasks;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Payslip
{
    public class YTDOTPaymentBreakdownDetailViewModel : BaseViewModel
    {
        public ICommand PrintPayslipCommand { get; set; }

        private YTDPayslipDetailHolder holder_;

        public YTDPayslipDetailHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IPayslipDataService service_;

        public YTDOTPaymentBreakdownDetailViewModel(IPayslipDataService service)
        {
            service_ = service;
        }

        public void Init(INavigation nav, long paysheetHeaderId, long profileId)
        {
            NavigationBack = nav;

            RetrieveRecord(paysheetHeaderId, profileId);
        }

        private async void RetrieveRecord(long recordId, long profileId)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Holder = await service_.GetPayslipYTDTemplateAsync(profileId, recordId);
                }
                catch (Exception ex)
                {
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void ExecutePrintPayslipCommand()
        {
            if (!IsBusy)
            {
                try
                {
                    await service_.PrintPayslip(Holder.Model.ProfileId, Holder.Model.PaysheetHeaderId);
                }
                catch (Exception ex)
                {
                    Error(false, ex.Message);
                }
            }
        }
    }
}