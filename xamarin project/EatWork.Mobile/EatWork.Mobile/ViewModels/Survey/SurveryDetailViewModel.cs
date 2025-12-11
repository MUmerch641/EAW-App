using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.FormHolder.Questionnaire;
using EatWork.Mobile.Models.Questionnaire;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Survey;
using EAW.API.DataContracts.Models;
using Mobile.Utils.ControlGenerator;
using Syncfusion.SfRotator.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Survey
{
    public class SurveryDetailViewModel : BaseViewModel
    {
        public ICommand PageAppearingCommand { get; set; }
        public ICommand ControlEventCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand CheckboxListEventCommand { get; set; }
        public ICommand RadioButtonEventCommand { get; set; }
        public ICommand CustomBackButtonCommand { get; set; }

        private SurveyHolder holder_;

        public SurveyHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private bool isEnabled_;

        public bool IsEnabled
        {
            get { return isEnabled_; }
            set { isEnabled_ = value; RaisePropertyChanged(() => IsEnabled); }
        }

        private readonly IGenerateControlService controlService_;
        private readonly ISurveyDataService service_;
        private readonly IDialogService dialogService_;

        public SurveryDetailViewModel()
        {
            controlService_ = AppContainer.Resolve<IGenerateControlService>();
            service_ = AppContainer.Resolve<ISurveyDataService>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, PulseSurveyList item)
        {
            NavigationBack = navigation;
            InitForm();
            SetupUIForm(item);
        }

        private void InitForm()
        {
            Holder = new SurveyHolder();
            NextCommand = new Command(Next);
            BackCommand = new Command(Back);
            ControlEventCommand = new Command<ControlDto>(ExecuteControlEventCommand);
            CheckboxListEventCommand = new Command<DataSourceQuestionDto>(ExecuteCheckboxListEventCommand);
            RadioButtonEventCommand = new Command(ExecuteRadioButtonEventCommand);
            CustomBackButtonCommand = new Command(async () => await ExecuteCustomBackButtonCommand());
            IsEnabled = true;
        }

        private async void SetupUIForm(PulseSurveyList record)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;

                    Holder.ControlList.Add(new BaseQControlDto()
                    {
                        ID = Constants.DEFAULTPAGE,
                        RotatorItem = new SurveyLandingPage(),
                        QuestionHeader = $"Welcome {PreferenceHelper.UserInfo().FirstName}!{Constants.NextLine}",
                        QuestionDetail = (!string.IsNullOrWhiteSpace(record.GreetingMessage) ? record.GreetingMessage : Messages.SurveyMessageText),
                    });

                    using (Dialogs.Loading())
                    {
                        await Task.Delay(100);

                        Holder.HasError = false;

                        var reponse = await service_.RetrievePulseSurvey(new ObservableCollection<BaseQControlDto>(), record.FormHeaderId, record.FormSurveyHistoryIdModel);

                        if (reponse.Count > 0)
                        {
                            Holder.FormHeaderId = record.FormHeaderId;
                            Holder.FormSurveyHistoryId = record.FormSurveyHistoryIdModel;
                            Holder.ToggleAnonimousOption = record.AllowAnonymous;
                            Holder.HasAnswer = reponse.FirstOrDefault().HasAnswer;

                            /*
                            Holder.FormHeaderId = reponse.FirstOrDefault().FormHeaderId;
                            Holder.FormSurveyHistoryId = reponse.FirstOrDefault().FormSurveyHistoryId;
                            */

                            foreach (var item in reponse)
                            {
                                Holder.ControlList.Add(item);

                                var view = new SurveryItemPage();

                                ICommand cmd = ControlEventCommand;
                                var controlType = (ControlTypeEnum)item.BaseQuestion.ControlTypeId;
                                var answer = "";
                                switch (controlType)
                                {
                                    case ControlTypeEnum.CheckboxOption:
                                        cmd = CheckboxListEventCommand;
                                        break;

                                    case ControlTypeEnum.RadioOption:
                                        cmd = RadioButtonEventCommand;
                                        break;

                                    case ControlTypeEnum.SwitchOption:
                                        answer = "No";
                                        break;

                                    default:
                                        answer = "N/A";
                                        break;
                                }

                                var genCtrl = await controlService_.ProcessControl(item, cmd);
                                if (genCtrl != null)
                                {
                                    view = new SurveryItemPage(genCtrl);
                                }

                                item.RotatorItem = view;
                                /*item.RotatorItem.BindingContext = item;*/

                                Holder.Answers.Add(new AnswerDto()
                                {
                                    BaseQuestion = item.BaseQuestion,
                                    ID = item.ID,
                                    ControlTypeId = item.BaseQuestion.ControlTypeId,
                                    FormQuestionId = item.BaseQuestion.FormQuestionId,
                                    Question = item.BaseQuestion.Question,
                                    QuestionName = item.BaseQuestion.QuestionName,
                                    ControlName = item.ControlName,
                                    QuestionHeader = item.QuestionHeader,
                                    QuestionDetail = item.QuestionDetail,
                                    Value = answer
                                });
                            }

                            if (!Holder.HasAnswer)
                            {
                                Holder.ControlList.Add(new BaseQControlDto()
                                {
                                    ID = $"{Constants.SUMMARYPAGE}",
                                    RotatorItem = new SummaryPage(),
                                });
                            }
                        }
                        else
                        {
                            ShowPage = false;
                        }

                        foreach (var item in Holder.ControlList)
                        {
                            if (item.ID == Constants.SUMMARYPAGE)
                                item.RotatorItem.BindingContext = this;
                            else
                                item.RotatorItem.BindingContext = item;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowPage = false;
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void Next(object obj)
        {
            var itemCount = (obj as SfRotator).ItemsSource.Count();
            if (await ValidateAndUpdateSelectedIndex(itemCount))
            {
                if (Holder.SelectedIndex == itemCount - 1 && !Holder.HasAnswer)
                    await SubmitSurvey();
            }
        }

        private async void Back(object obj)
        {
            var itemCount = (obj as SfRotator).ItemsSource.Count();
            if (await ValidateAndUpdateSelectedIndex(itemCount, true))
            {
                if (Holder.SelectedIndex == itemCount - 1)
                {
                    Holder.SelectedIndex = 1;
                    Holder.Current = 1;
                    UpdateContentName(itemCount);
                }
            }
        }

        private async Task<bool> ValidateAndUpdateSelectedIndex(int itemCount, bool isBack = false)
        {
            if (!isBack)
            {
                var item = Holder.ControlList.ElementAt(Holder.SelectedIndex);
                if (item != null && item.BaseQuestion != null)
                {
                    /*if (item.BaseQuestion.IsRequired && string.IsNullOrWhiteSpace(Holder.InputValue))*/
                    if (item.BaseQuestion.IsRequired && string.IsNullOrWhiteSpace(Holder.InputValue) && item.BaseQuestion.ControlTypeId != (short)ControlTypeEnum.SwitchOption)
                    {
                        await dialogService_.AlertAsync("Please input value");
                        Holder.HasError = true;

                        /*
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await dialogService_.ToastMessageAsync(ToastType.Error, "Please input value");
                        });
                        */
                    }
                }
            }

            if (!Holder.HasError)
            {
                if (Holder.SelectedIndex == itemCount - 2)
                {
                    ConsolidateCheckbox();
                }

                if (Holder.SelectedIndex >= itemCount - 1)
                {
                    return true;
                }

                if (isBack)
                    Holder.SelectedIndex--;
                else
                    Holder.SelectedIndex++;

                UpdateContentName(itemCount);
            }

            return false;
        }

        private void ExecuteControlEventCommand(ControlDto item)
        {
            if (item != null)
            {
                Holder.HasError = Holder.ValidateAnswer(item);
                item.HasError = Holder.HasError;
                Holder.InputValue = item.Label;

                if (!Holder.HasError)
                    Holder.Answers = Holder.CollectAnswer(item, Holder.Answers);
            }
        }

        private void ExecuteRadioButtonEventCommand(object item)
        {
            if (item != null)
            {
                var control = item as CustomSfRadioButton;
                var dto = new ControlDto()
                {
                    ID = control.UniqueId,
                    ControlTypeId = control.ControlTypeId,
                    Value = control.Text,
                };

                Holder.Answers = Holder.CollectAnswer(dto, Holder.Answers);
            }
        }

        private void ExecuteCheckboxListEventCommand(DataSourceQuestionDto item)
        {
            if (item != null)
            {
                var exist = Holder.CheckboxListAnswers.FirstOrDefault(p => p.ID == item.ID && p.DisplayText == item.DisplayText);

                if (exist == null)
                    Holder.CheckboxListAnswers.Add(item);
                else
                {
                    if (exist != null && !item.IsChecked)
                        Holder.CheckboxListAnswers.Remove(exist);
                }
            }

            Holder.ListHolder = Holder.CheckboxListAnswers
                .GroupBy(p => p.DisplayId)
                .Select(p => new ListHolderDto
                {
                    FormId = p.Key,
                    CommaSeparatedValue = string.Join(",", p.Select(x => x.DisplayText))
                })
                .ToList();
        }

        public void UpdateContentName(int itemCount)
        {
            if (Holder.SelectedIndex >= itemCount - 2)
                Holder.Current = itemCount - 2;
            else
                Holder.Current = Holder.SelectedIndex;

            Holder.TitleContent = $"{Holder.Current} of {itemCount - 2}";
            Holder.TotalItemCount = itemCount - 2;

            Holder.InputValue = String.Empty;
        }

        private void ConsolidateCheckbox()
        {
            foreach (var item in Holder.ListHolder)
            {
                var dto = new ControlDto()
                {
                    ID = item.FormId,
                    Value = item.CommaSeparatedValue,
                    ControlTypeId = (int)ControlTypeEnum.CheckboxOption,
                };

                ExecuteControlEventCommand(dto);
            }
        }

        private async Task SubmitSurvey()
        {
            if (Holder.Answers.Count > 0)
            {
                Holder = await service_.SubmitAnswer(Holder);

                if (Holder.IsSuccess)
                {
                    Success(true, $"{Constants.NextLine}{(!string.IsNullOrWhiteSpace(Holder.EndMessage) ? Holder.EndMessage : Messages.SurveyEndingMessage)}", "Thank you!");
                    await NavigationService.PopPageAsync();
                }
            }
        }

        private async Task ExecuteCustomBackButtonCommand()
        {
            try
            {
                if (!IsBusy)
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        Application.Current.MainPage = new NavigationPage(new DashboardPage());
                        //await NavigationService.PopToRootAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }
        }
    }
}