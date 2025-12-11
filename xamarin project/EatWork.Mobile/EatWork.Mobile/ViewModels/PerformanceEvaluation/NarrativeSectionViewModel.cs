using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.PerformanceEvaluation
{
    public class NarrativeSectionViewModel : BaseViewModel
    {
        public ICommand UnfocusedCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SubmitRecordCommand { get; set; }

        private PEFormHolder holder_;

        public PEFormHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IDialogService dialogService_;
        private readonly IPEFormDataService service_;

        private bool forSubmission_;

        public bool ForSubmission
        {
            get { return forSubmission_; }
            set { forSubmission_ = value; RaisePropertyChanged(() => ForSubmission); }
        }

        private bool canSave_;

        public bool CanSave
        {
            get { return canSave_; }
            set { canSave_ = value; RaisePropertyChanged(() => CanSave); }
        }

        private bool canSubmit_;

        public bool CanSubmit
        {
            get { return canSubmit_; }
            set { canSubmit_ = value; RaisePropertyChanged(() => CanSubmit); }
        }

        public NarrativeSectionViewModel()
        {
            dialogService_ = AppContainer.Resolve<IDialogService>();
            service_ = AppContainer.Resolve<IPEFormDataService>();
        }

        public void Init(INavigation nav, PEFormHolder holder)
        {
            NavigationBack = nav;
            Holder = holder;

            UnfocusedCommand = new Command<NarrativeSectionDto>(ExecuteUnfocusedCommand);
            SaveCommand = new Command(async () => await ExecuteSaveCommand()); ;
            SubmitRecordCommand = new Command(async () => await ExecuteSubmitRecordCommand());

            ForSubmission = holder.ForSubmission;
            CanSubmit = holder.CanSubmit;
            CanSave = holder.CanSave;
        }

        private void ExecuteUnfocusedCommand(NarrativeSectionDto item)
        {
            try
            {
                if (!IsBusy)
                {
                    if (item != null)
                    {
                        item.HasError = false;

                        //display
                        var index = Holder.Narratives.ToList().FindIndex(x => x.QuestionId == item.QuestionId && x.EvaluationId == item.EvaluationId);
                        var exst = Holder.Narratives.FirstOrDefault(x => x.QuestionId == item.QuestionId && x.EvaluationId == item.EvaluationId);

                        if (string.IsNullOrWhiteSpace(item.Answer) && item.IsRequired)
                            item.HasError = true;

                        Holder.Narratives.Remove(exst);
                        Holder.Narratives.Insert(index, item);

                        Holder = Holder;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} - {ex.Message}");
            }
        }

        private async Task ExecuteSaveCommand()
        {
            try
            {
                Holder = await service_.ValidatePANarrativeSection(Holder);

                if (!Holder.HasError)
                {
                    if (await dialogService_.ConfirmDialogAsync(Messages.Save))
                        Holder = await service_.SavePANarrativeSection(Holder);

                    if (Holder.IsSuccess)
                        Success(true, Messages.RecordSaved);
                    else
                        Error(false, "Error occured while saving the record");
                }
                else
                    await dialogService_.AlertAsync("Please fill out required fields.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} - {ex.Message}");
                Error(false, ex.Message);
            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private async Task ExecuteSubmitRecordCommand()
        {
            try
            {
                if (Holder.ForSubmission)
                {
                    if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                    {
                        Holder = await service_.SubmitPerformanceEvaluation(Holder);
                        if (Holder.IsSuccess)
                        {
                            Success(true, Messages.RecordSaved);
                            await NavigationService.PopToRootAsync();
                        }
                        else
                            Error(false, "Error occured while submitting the record");
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
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                Error(false, ex.Message);
            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        protected override async void BackItemPage()
        {
            if (await dialogService_.ConfirmDialogAsync("Move to Questionnaire?"))
                base.BackItemPage();
        }
    }
}