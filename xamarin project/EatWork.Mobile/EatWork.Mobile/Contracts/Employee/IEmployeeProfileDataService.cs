using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Profile;
using Syncfusion.ListView.XForms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IEmployeeProfileDataService
    {
        //retrieve employee profile by id

        //retrieve employee profile category
        Task<ObservableCollection<MenuItemModel>> InitEmployeeCategory();

        //sub-menu list for other employee profile categories
        Task<ObservableCollection<MenuItemModel>> InitEmployeeSubCategory(int groupId = 0);

        //personal details
        Task<ProfileHolder> InitPersonalDetails(long profileId);

        //family background
        Task<ObservableCollection<FamilyBackgroundListHolder>> RetrieveFamilyBackgroundList(int listcount, int count, ObservableCollection<FamilyBackgroundListHolder> list, long profileId);

        Task<SfListView> InitListViewFamilyBackground(SfListView listview, bool isAsceding = false);

        //educational background
        Task<ObservableCollection<EducationalBackgroundListHolder>> RetrieveEducationalBackgroundList(int listcount, int count, ObservableCollection<EducationalBackgroundListHolder> list, long profileId);

        Task<SfListView> InitListViewEducationalBackground(SfListView listview, bool isAsceding = false);

        //employment information
        Task<EmploymentInformationHolder> InitEmploymentInformation(long profileId);

        Task<Xamarin.Forms.ImageSource> GetProfileImage(long profileId = 0);
    }
}