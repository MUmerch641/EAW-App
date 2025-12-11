using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Validations;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.PerformanceEvaluation
{
    public class InputRatingViewModel : BaseViewModel
    {
        public ICommand CloseModalCommand { get; set; }
        public ICommand SaveRatingCommand { get; set; }

        private ObjectiveDetailDto data_;

        public ObjectiveDetailDto Data
        {
            get { return data_; }
            set { data_ = value; RaisePropertyChanged(() => Data); }
        }

        private ValidatableObject<string> review_;

        public ValidatableObject<string> EmployeeReview
        {
            get { return review_; }
            set { review_ = value; RaisePropertyChanged(() => EmployeeReview); }
        }

        private ValidatableObject<string> actual_;

        public ValidatableObject<string> Actual
        {
            get { return actual_; }
            set { actual_ = value; RaisePropertyChanged(() => Actual); }
        }

        private ValidatableObject<decimal> employeeRating_;

        public ValidatableObject<decimal> EmployeeRating
        {
            get { return employeeRating_; }
            set { employeeRating_ = value; RaisePropertyChanged(() => EmployeeRating); }
        }

        public InputRatingViewModel()
        {
            Data = new ObjectiveDetailDto();
            EmployeeReview = new ValidatableObject<string>();
            Actual = new ValidatableObject<string>();
            EmployeeRating = new ValidatableObject<decimal>();
        }

        public void Init(ObjectiveDetailDto item)
        {
            Data = item;
            CloseModalCommand = new Command(async () => await PopupNavigation.Instance.PopAsync(true));
            SaveRatingCommand = new Command(ExecuteSaveRatingCommand);

            SetUpData();
        }

        private void SetUpData()
        {
            if (Data != null)
            {
                EmployeeReview.Value = Data.EmployeeReview;
                Actual.Value = Data.Actual;
                EmployeeRating.Value = Data.EmployeeRating;
            }
        }

        private async void ExecuteSaveRatingCommand()
        {
            if (IsValid())
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    MessagingCenter.Send(this, "SaveRatingCompleted", Data);
                    await PopupNavigation.Instance.PopAsync(true);
                }
            }
        }

        private bool IsValid()
        {
            Data.EmployeeReview = EmployeeReview.Value;
            Data.Actual = Actual.Value;
            Data.EmployeeRating = EmployeeRating.Value;

            return true;
        }
    }
}