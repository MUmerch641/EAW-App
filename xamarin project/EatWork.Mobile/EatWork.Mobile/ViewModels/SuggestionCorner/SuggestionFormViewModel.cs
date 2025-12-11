using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models.FormHolder.SuggestionCorner;
using EatWork.Mobile.Views.SuggestionCorner;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.ViewModels.SuggestionCorner
{
    public class SuggestionFormViewModel : BaseViewModel
    {
        #region commands

        public ICommand ResetFormCommand { get; set; }
        public ICommand SubmitCommand { get; set; }
        public ICommand ToggleSuggestionCategoryCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        private readonly IDialogService dialogService_;
        private readonly ISuggestionFormDataService service_;

        private FormHolder holder_;

        public FormHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        public SuggestionFormViewModel()
        {
            dialogService_ = AppContainer.Resolve<IDialogService>();
            service_ = AppContainer.Resolve<ISuggestionFormDataService>();
        }

        public void Init(INavigation navigation, R.Models.SuggestionListDto item)
        {
            NavigationBack = navigation;
            Holder = new FormHolder();

            SubmitCommand = new Command(async () => await ExecuteSubmitCommand());
            ResetFormCommand = new Command(ExecuteResetFormCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);
            ToggleSuggestionCategoryCommand = new Command(ExecuteToggleSuggestionCategoryCommand);

            InitForm();
            InitHelpers();
        }

        private async void InitForm()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder.Categories = await service_.GetCategories();
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

        private void InitHelpers()
        {
            MessagingCenter.Subscribe<SuggestionFormPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private async Task ExecuteSubmitCommand()
        {
            try
            {
                Holder = await service_.SaveRecord(Holder);

                if (Holder.Success)
                {
                    Success(true, Messages.RecordSaved);
                    await NavigationService.PopToRootAsync();
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
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private async void ExecuteToggleSuggestionCategoryCommand()
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new CategoryModalPage());
        }

        private void ExecuteResetFormCommand()
        {
            Holder.Suggestions.Value = string.Empty;
            Holder.Category.Value = string.Empty;
            Holder.SelectedCategory = null;
        }

        private void ExecutePageAppearingCommand()
        {
            MessagingCenter.Subscribe<CategoryModalViewModel, R.Models.SuggestionCategory>(this, "SuggestionCategorySaved", (s, param) =>
            {
                if (param != null)
                {
                    InitForm();
                    var item = Holder.Categories.Where(x => x.Id == param.SuggestionCategoryId).FirstOrDefault();

                    if (item != null)
                        Holder.SelectedCategory = item;
                }
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<CategoryModalViewModel, R.Models.SuggestionCategory>(this, "SuggestionCategorySaved");
        }

        protected override async void BackItemPage()
        {
            if (!string.IsNullOrWhiteSpace(Holder.SelectedCategory.Value) ||
                !string.IsNullOrWhiteSpace(Holder.Suggestions.Value))
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.LEAVEPAGE))
                {
                    base.BackItemPage();
                }
            }
            else
            {
                base.BackItemPage();
            }
        }
    }
}