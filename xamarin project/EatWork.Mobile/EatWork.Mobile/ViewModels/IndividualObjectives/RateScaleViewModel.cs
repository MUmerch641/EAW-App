using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Validations;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.IndividualObjectives
{
    public class RateScaleViewModel : BaseViewModel
    {
        public ICommand CloseModalCommand { get; set; }
        public ICommand AddRatingCommand { get; set; }
        public ICommand DeleteRateCommand { get; set; }

        private RateScaleDto model_;

        public RateScaleDto Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        private ValidatableObject<decimal> minRate_;

        public ValidatableObject<decimal> MinRate
        {
            get { return minRate_; }
            set { minRate_ = value; RaisePropertyChanged(() => MinRate); }
        }

        private ValidatableObject<decimal> maxRate_;

        public ValidatableObject<decimal> MaxRate
        {
            get { return maxRate_; }
            set { maxRate_ = value; RaisePropertyChanged(() => MaxRate); }
        }

        private ValidatableObject<decimal> rating_;

        public ValidatableObject<decimal> Rating
        {
            get { return rating_; }
            set { rating_ = value; RaisePropertyChanged(() => Rating); }
        }

        private ValidatableObject<string> criteria_;

        public ValidatableObject<string> Criteria
        {
            get { return criteria_; }
            set { criteria_ = value; RaisePropertyChanged(() => Criteria); }
        }

        private bool isEnabled_;

        public bool IsEnabled
        {
            get { return isEnabled_; }
            set { isEnabled_ = value; RaisePropertyChanged(() => IsEnabled); }
        }

        private bool isEditable_;

        public bool IsEditable
        {
            get { return isEditable_; }
            set { isEditable_ = value; RaisePropertyChanged(() => IsEditable); }
        }

        private bool isExistingItem_;

        public bool IsExistingItem
        {
            get { return isExistingItem_; }
            set { isExistingItem_ = value; RaisePropertyChanged(() => IsExistingItem); }
        }

        public long CriteriaId { get; set; }
        public long TempId { get; set; }

        public RateScaleViewModel()
        {
        }

        public void Init(RateScaleDto item = null, bool isEditable = true)
        {
            IsEnabled = true;
            IsEditable = true;
            IsExistingItem = false;
            CriteriaId = 0;
            Model = new RateScaleDto();
            MinRate = new ValidatableObject<decimal>();
            MaxRate = new ValidatableObject<decimal>();
            Rating = new ValidatableObject<decimal>();
            Criteria = new ValidatableObject<string>();
            CloseModalCommand = new Command(async () => await PopupNavigation.Instance.PopAsync(true));
            AddRatingCommand = new Command(async () => await ExecuteCommand());
            DeleteRateCommand = new Command(async () => await ExecuteCommand(true));

            InitForm(item, isEditable);
        }

        private async Task ExecuteCommand(bool isDelete = false)
        {
            if (Execute(isDelete))
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    MessagingCenter.Send(this, "RateScaleCompleted", Model);
                    await PopupNavigation.Instance.PopAsync(true);
                }
            }
        }

        private async void InitForm(RateScaleDto item, bool isEditable)
        {
            if (item != null)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    MinRate.Value = item.Min;
                    MaxRate.Value = item.Max;
                    Rating.Value = item.Rating;
                    Criteria.Value = item.Criteria;
                    CriteriaId = item.CriteriaId;
                    IsEditable = isEditable;
                    IsExistingItem = true;
                    TempId = item.TempId;
                }
            }
        }

        private bool Execute(bool isDelete = false)
        {
            Model = new RateScaleDto()
            {
                Criteria = Criteria.Value,
                Max = MaxRate.Value,
                Min = MinRate.Value,
                Rating = Rating.Value,
                CriteriaId = CriteriaId,
                TempId = TempId,
                IsDelete = isDelete,
            };

            if (isDelete)
                return true;
            else
                return IsValid();
        }

        private bool IsValid()
        {
            MinRate.Validations.Clear();
            /*
            MinRate.Validations.Add(new NumberRule<decimal>()
            {
                ValidationMessage = string.Empty,
            });
            */

            MaxRate.Validations.Clear();
            /*
            MaxRate.Validations.Add(new NumberRule<decimal>()
            {
                ValidationMessage = string.Empty,
            });
            */

            Rating.Validations.Clear();
            Rating.Validations.Add(new NumberRule<decimal>()
            {
                ValidationMessage = string.Empty,
            });

            Criteria.Validations.Clear();
            Criteria.Validations.Add(new IsNotNullOrEmptyRule<string>()
            {
                ValidationMessage = string.Empty,
            });

            MinRate.Validate();
            MaxRate.Validate();
            Rating.Validate();
            Criteria.Validate();

            return MinRate.IsValid &&
                   MaxRate.IsValid &&
                   Rating.IsValid &&
                   Criteria.IsValid;
        }
    }
}