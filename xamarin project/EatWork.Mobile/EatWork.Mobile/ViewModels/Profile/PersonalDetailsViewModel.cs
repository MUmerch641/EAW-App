using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Profile;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class PersonalDetailsViewModel : BaseViewModel
    {
        private ProfileHolder profile_;

        public ProfileHolder Profile
        {
            get { return profile_; }
            set { profile_ = value; RaisePropertyChanged(() => Profile); }
        }

        private readonly IEmployeeProfileDataService employeeDataService_;

        public PersonalDetailsViewModel(IEmployeeProfileDataService employeeDataService)
        {
            employeeDataService_ = employeeDataService;
        }

        public void InitPersonalDetails(long profileId, INavigation navigation)
        {
            NavigationBack = navigation;
            Profile = new ProfileHolder();
            RetrievePersonalDetails(profileId);
        }

        private async void RetrievePersonalDetails(long profileId)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Profile = await employeeDataService_.InitPersonalDetails(profileId);
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
    }
}