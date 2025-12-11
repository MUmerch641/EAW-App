using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Validations;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using R = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.ViewModels.SuggestionCorner
{
    public class CategoryModalViewModel : BaseViewModel
    {
        public ICommand CloseModalCommand { get; set; }
        public ICommand AddRequestCommand { get; set; }

        private ValidatableObject<string> category_;

        public ValidatableObject<string> Category
        {
            get { return category_; }
            set { category_ = value; RaisePropertyChanged(() => Category); }
        }

        public R.SuggestionCategory SuggestionCategory { get; set; }

        private readonly ISuggestionFormDataService service_;

        public CategoryModalViewModel()
        {
            service_ = AppContainer.Resolve<ISuggestionFormDataService>();
        }

        public void Init()
        {
            Category = new ValidatableObject<string>();
            SuggestionCategory = new R.SuggestionCategory();

            CloseModalCommand = new Command(async () => await PopupNavigation.Instance.PopAsync(true));
            AddRequestCommand = new Command(ExecuteAddRequestCommand);
        }

        private async void ExecuteAddRequestCommand()
        {
            try
            {
                if (IsValid())
                {
                    SuggestionCategory = await service_.SaveCategory(Category.Value);

                    if (SuggestionCategory.SuggestionCategoryId > 0)
                    {
                        MessagingCenter.Send(this, "SuggestionCategorySaved", SuggestionCategory);
                        //await PopupNavigation.Instance.PopAsync(true);
                    }
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                var list = new ObservableCollection<string>(ex.Model.Errors.Values.Select(p => p[0]));
                Error(results: list, title: ex.Model.Title.ToUpper(), autoHide: false);
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private bool IsValid()
        {
            Category.Validations.Clear();
            Category.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            Category.Validate();

            return Category.IsValid;
        }
    }
}