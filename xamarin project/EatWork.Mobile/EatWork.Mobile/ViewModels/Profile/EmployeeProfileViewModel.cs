using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Profile;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class EmployeeProfileViewModel : BaseViewModel
    {
        private readonly IEmployeeProfileDataService employeeDataService_;

        #region properties

        private readonly int totalItems_ = 10;
        private SfListView sfListView_;

        public SfListView SfListView
        {
            get { return sfListView_; }
            set { sfListView_ = value; RaisePropertyChanged(() => SfListView); }
        }

        private ObservableCollection<FamilyBackgroundListHolder> familyBackgroundList_;

        public ObservableCollection<FamilyBackgroundListHolder> FamilyBackgroundList
        {
            get { return familyBackgroundList_; }
            set { familyBackgroundList_ = value; RaisePropertyChanged(() => FamilyBackgroundList); }
        }

        #endregion properties

        public EmployeeProfileViewModel(IEmployeeProfileDataService employeeDataService)
        {
            employeeDataService_ = employeeDataService;
        }

        #region family background

        public void InitFamilyBackground(SfListView familyListView, long recordId, INavigation navigation)
        {
            NavigationBack = navigation;
            FamilyBackgroundList = new ObservableCollection<FamilyBackgroundListHolder>();
            SfListView = familyListView;

            LoadFamilyBackgroundListItems(recordId);
        }

        private async void LoadFamilyBackgroundListItems(long recordId)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(100);
                    var list = employeeDataService_.RetrieveFamilyBackgroundList(FamilyBackgroundList.Count, totalItems_, FamilyBackgroundList, recordId);
                    var listview = employeeDataService_.InitListViewFamilyBackground(SfListView);

                    await Task.WhenAll(list, listview);

                    SfListView = listview.Result;
                    FamilyBackgroundList = list.Result;

                    await Task.WhenAll();
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

        #endregion family background
    }
}