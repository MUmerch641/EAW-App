using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Payslip;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Payslip
{
    internal class MyPayslipDetailViewModel : BaseViewModel
    {
        public ICommand PrintPayslipCommand { get; set; }

        private PayslipDetailHolder holder_;

        public PayslipDetailHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IPayslipDataService service_;

        public MyPayslipDetailViewModel(IPayslipDataService service)
        {
            service_ = service;
        }

        public void Init(INavigation navigation, long recordId, long profileId)
        {
            NavigationBack = navigation;

            InitForm();
            RetrieveRecord(recordId, profileId);
        }

        private void InitForm()
        {
            Holder = new PayslipDetailHolder();

            PrintPayslipCommand = new Command(ExecutePrintPayslipCommand);
        }

        private async void RetrieveRecord(long recordId, long profileId)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Holder = await service_.GetPayslipDetailAsync(profileId, recordId);
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