using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Profile;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class EmploymentInformationViewModel : BaseViewModel
    {
        private EmploymentInformationHolder model_;

        public EmploymentInformationHolder Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        private readonly IEmployeeProfileDataService employeeDataService_;

        public EmploymentInformationViewModel(IEmployeeProfileDataService employeeDataService)
        {
            employeeDataService_ = employeeDataService;
        }

        public void InitEmploymentInformation(long profileId, INavigation navigation)
        {
            NavigationBack = navigation;
            Model = new EmploymentInformationHolder();

            RetrieveEmploymentInformation(profileId);
        }

        private async void RetrieveEmploymentInformation(long profileId)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(1000);

                    Model = await employeeDataService_.InitEmploymentInformation(profileId);
                }
                catch (Exception ex)
                {
                    await Dialogs.AlertAsync(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}