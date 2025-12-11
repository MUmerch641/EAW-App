using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.Questionnaire;
using EatWork.Mobile.Utils;
using Mobile.Utils.ControlGenerator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CONTROL = Mobile.Utils.ControlGenerator;

namespace EatWork.Mobile.Models.FormHolder.Questionnaire
{
    public class SurveyHolder : ExtendedBindableObject
    {
        public SurveyHolder()
        {
            ControlList = new ObservableCollection<BaseQControlDto>();
            TotalItemCount = 0;
            Answers = new ObservableCollection<AnswerDto>();
            CheckboxListAnswers = new ObservableCollection<DataSourceQuestionDto>();
            HasError = true;
            Current = 0;
            ListHolder = new List<ListHolderDto>();
            IsSuccess = false;
            HasAnswer = false;
            ToggleAnonimousOption = false;
            SubmitAnonimously = false;
            IsRequired = false;
            EnableButton = true;
        }

        private ObservableCollection<BaseQControlDto> controlList_;

        public ObservableCollection<BaseQControlDto> ControlList
        {
            get { return controlList_; }
            set { controlList_ = value; RaisePropertyChanged(() => ControlList); }
        }

        private bool hasError_;

        public bool HasError
        {
            get { return hasError_; }
            set { hasError_ = value; RaisePropertyChanged(() => HasError); }
        }

        private bool isDefaultPage_ = true;

        public bool IsDefaultPage
        {
            get { return isDefaultPage_; }
            set { isDefaultPage_ = value; RaisePropertyChanged(() => IsDefaultPage); }
        }

        private bool isDetail_ = false;

        public bool IsDetail
        {
            get { return isDetail_; }
            set { isDetail_ = value; RaisePropertyChanged(() => IsDetail); }
        }

        private string titleContent_;

        public string TitleContent
        {
            get { return titleContent_; }
            set { titleContent_ = value; RaisePropertyChanged(() => TitleContent); }
        }

        private int current_;

        public int Current
        {
            get { return current_; }
            set { current_ = value; RaisePropertyChanged(() => Current); }
        }

        private int totalItemCount_;

        public int TotalItemCount
        {
            get { return totalItemCount_; }
            set { totalItemCount_ = value; RaisePropertyChanged(() => TotalItemCount); }
        }

        private int selectedIndex_;

        public int SelectedIndex
        {
            get
            {
                return selectedIndex_;
            }

            set
            {
                if (selectedIndex_ == value)
                {
                    return;
                }

                selectedIndex_ = value;
                RaisePropertyChanged(() => SelectedIndex);
            }
        }

        private string nextButtonText_ = "Next";

        public string NextButtonText
        {
            get
            {
                return nextButtonText_;
            }

            set
            {
                if (nextButtonText_ == value)
                {
                    return;
                }

                nextButtonText_ = value;
                RaisePropertyChanged(() => NextButtonText);
            }
        }

        private string backButtonText = "Back";

        public string BackButtonText
        {
            get
            {
                return backButtonText;
            }

            set
            {
                if (backButtonText == value)
                {
                    return;
                }

                backButtonText = value;
                RaisePropertyChanged(() => BackButtonText);
            }
        }

        private string defaultPageButtonText_ = "START SURVEY";

        public string DefaultPageButtonText
        {
            get { return defaultPageButtonText_; }
            set { defaultPageButtonText_ = value; RaisePropertyChanged(() => DefaultPageButtonText); }
        }

        private ObservableCollection<AnswerDto> answers_;

        public ObservableCollection<AnswerDto> Answers
        {
            get { return answers_; }
            set { answers_ = value; RaisePropertyChanged(() => Answers); }
        }

        private ObservableCollection<DataSourceQuestionDto> checkboxListAnswers_;

        public ObservableCollection<DataSourceQuestionDto> CheckboxListAnswers
        {
            get { return checkboxListAnswers_; }
            set { checkboxListAnswers_ = value; RaisePropertyChanged(() => CheckboxListAnswers); }
        }

        public ObservableCollection<AnswerDto> CollectAnswer(ControlDto item, ObservableCollection<AnswerDto> answers)
        {
            var exist = answers.FirstOrDefault(p => p.ID == item.ID);
            var index = answers.IndexOf(exist);
            var controlType = (ControlTypeEnum)item.ControlTypeId;
            var answer = string.Empty;

            switch (controlType)
            {
                case ControlTypeEnum.Textbox:
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        answer = item.Value;
                    }
                    break;

                case ControlTypeEnum.MultilineTextbox:
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        answer = item.Value;
                    }
                    break;

                case ControlTypeEnum.Dropdown:
                    if (item.SelectedItem.ID > 0)
                    {
                        answer = item.SelectedItem.DisplayText;
                    }
                    break;

                case ControlTypeEnum.MultiselectDropdown:
                    if (item.MultiSelectOptions.Count > 0)
                    {
                        var selected = item.MultiSelectOptions.ToList().Cast<DataSourceDto>();
                        /*answer = string.Join(",", selected.Select(x => x.ID));*/
                        answer = string.Join(",", selected.Select(x => x.DisplayText));
                    }
                    /*
                    if (!string.IsNullOrWhiteSpace(item.SelectedItem.DisplayText))
                    {
                        answer = item.SelectedItem.DisplayText;
                    }
                    */
                    break;

                case ControlTypeEnum.DatePicker:
                    answer = (item.DateValue.Value > Constants.NullDate ? item.DateValue.Value.ToString(Constants.DateFormatMMDDYYYY) : "");
                    break;

                case ControlTypeEnum.DateRangePicker:
                    var start = "";
                    var end = "";

                    if (item.StartDateValue.Value > Constants.NullDate)
                        start = item.StartDateValue.GetValueOrDefault().ToString(Constants.DateFormatMMDDYYYY);

                    if (item.EndDateValue.Value > Constants.NullDate)
                        end = item.EndDateValue.GetValueOrDefault().ToString(Constants.DateFormatMMDDYYYY);

                    answer = $"{start} - {end}";
                    break;

                case ControlTypeEnum.TimePicker:
                    answer = (item.TimeValue.HasValue ? new DateTime(item.TimeValue.Value.Ticks).ToString(Constants.TimeFormatHHMMTT) : "");
                    break;

                case ControlTypeEnum.TimeRangePicker:
                    var startTime = "";
                    var endTime = "";

                    if (item.StartTimeValue.HasValue)
                        startTime = new DateTime(item.StartTimeValue.Value.Ticks).ToString(Constants.TimeFormatHHMMTT);

                    if (item.EndTimeValue.HasValue)
                        endTime = new DateTime(item.EndTimeValue.Value.Ticks).ToString(Constants.TimeFormatHHMMTT);

                    answer = $"{startTime} - {endTime}";
                    break;

                case ControlTypeEnum.CheckboxOption:
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        answer = item.Value;
                    }
                    break;

                case ControlTypeEnum.RadioOption:
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        answer = item.Value;
                    }
                    break;

                case ControlTypeEnum.SwitchOption:
                    answer = item.IsChecked.ToString();
                    break;

                case ControlTypeEnum.StarRating:
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        answer = item.Value;
                    }
                    break;
            }

            exist.Value = answer;
            answers[index] = exist;

            return answers;
        }

        public bool ValidateAnswer(ControlDto item)
        {
            var retVal = false;

            var controlType = (ControlTypeEnum)item.ControlTypeId;

            switch (controlType)
            {
                case ControlTypeEnum.Textbox:
                    //if (string.IsNullOrWhiteSpace(item.Value))
                    //    retVal = true;
                    retVal = false;
                    break;

                case ControlTypeEnum.MultilineTextbox:
                    //if (string.IsNullOrWhiteSpace(item.Value))
                    //    retVal = true;
                    retVal = false;
                    break;

                case ControlTypeEnum.Dropdown:
                    //if (string.IsNullOrWhiteSpace(item.SelectedItem.DisplayText))
                    //    retVal = true;
                    retVal = false;
                    break;

                case ControlTypeEnum.MultiselectDropdown:
                    retVal = false;
                    break;

                case ControlTypeEnum.DatePicker:
                    retVal = false;
                    break;

                case ControlTypeEnum.DateRangePicker:
                    retVal = false;
                    break;

                case ControlTypeEnum.TimePicker:
                    retVal = false;
                    break;

                case ControlTypeEnum.TimeRangePicker:
                    retVal = false;
                    break;

                case ControlTypeEnum.CheckboxOption:
                    retVal = false;
                    break;

                case ControlTypeEnum.RadioOption:
                    retVal = false;
                    break;

                case ControlTypeEnum.SwitchOption:
                    retVal = false;
                    break;

                case ControlTypeEnum.StarRating:
                    retVal = false;
                    break;

                default:
                    retVal = false;
                    break;
            }

            return retVal;
        }

        public List<ListHolderDto> ListHolder { get; set; }

        public long FormHeaderId { get; set; }
        public long FormSurveyHistoryId { get; set; }
        public bool ToggleAnonimousOption { get; set; }
        public bool SubmitAnonimously { get; set; }
        public bool IsSuccess { get; set; }

        private string startMessage_;

        public string StartMessage
        {
            get { return startMessage_; }
            set { startMessage_ = value; RaisePropertyChanged(() => StartMessage); }
        }

        private string endMessage_;

        public string EndMessage
        {
            get { return startMessage_; }
            set { startMessage_ = value; RaisePropertyChanged(() => EndMessage); }
        }

        private bool hasAnswer_;

        public bool HasAnswer
        {
            get { return hasAnswer_; }
            set { hasAnswer_ = value; RaisePropertyChanged(() => HasAnswer); }
        }

        private bool isRequired_;

        public bool IsRequired
        {
            get { return isRequired_; }
            set { isRequired_ = value; RaisePropertyChanged(() => IsRequired); }
        }

        private string inputValue_;

        public string InputValue
        {
            get { return inputValue_; }
            set { inputValue_ = value; RaisePropertyChanged(() => InputValue); }
        }

        private bool enabledButton_;

        public bool EnableButton
        {
            get { return enabledButton_; }
            set { enabledButton_ = value; RaisePropertyChanged(() => EnableButton); }
        }

    }

    public class AnswerDto
    {
        public AnswerDto()
        {
            BaseQuestion = new BaseQuestion();
        }

        public long FormQuestionId { get; set; }
        public string QuestionName { get; set; }
        public int ControlTypeId { get; set; }
        public string Question { get; set; }
        public string Value { get; set; }
        public string ID { get; set; }
        public CONTROL.ControlDto ControlDto { get; set; }
        public BaseQuestion BaseQuestion { get; set; }
        public string QuestionHeader { get; set; }
        public string QuestionDetail { get; set; }
        public string ControlName { get; set; }
    }

    public class ListHolderDto
    {
        public string FormId { get; set; }
        public string CommaSeparatedValue { get; set; }
    }
}