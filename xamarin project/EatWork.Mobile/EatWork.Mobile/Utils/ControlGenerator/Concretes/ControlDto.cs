using EatWork.Mobile.Models.Questionnaire;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;

namespace Mobile.Utils.ControlGenerator
{
    public class DataSourceDto
    {
        public long ID { get; set; }
        public string DisplayText { get; set; }
        public bool IsChecked { get; set; }
    }

    public class DataSourceQuestionDto : DataSourceDto
    {
        public DataSourceQuestionDto()
        {
            BaseQuestion = new BaseQuestion();
        }

        public int ControlTypeId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string DisplayId { get; set; }
        public long FormQuestionId { get; set; }
        public BaseQuestion BaseQuestion { get; set; }
    }

    public class ControlDto : ExtendedBindableObject
    {
        public ControlDto()
        {
            /*
            TimeValue = DateTime.UtcNow.TimeOfDay;
            DateValue = DateTime.UtcNow.Date;
            StartDateValue = DateTime.UtcNow.Date;
            EndDateValue = DateTime.UtcNow.Date;
            StartTimeValue = DateTime.UtcNow.TimeOfDay;
            EndTimeValue = DateTime.UtcNow.TimeOfDay;
            */
            Value = string.Empty;
            SelectedItem = new DataSourceDto();
            SelectedOption = new SelectedOptionDto();
            DataSource = new ObservableCollection<DataSourceDto>();
            ControlTypeId = 0;
            IsChecked = false;
            Question = new BaseQuestion();
            HasError = false;
            SelectedRadioOption = string.Empty;
            MultiSelectOptions = new ObservableCollection<object>();
            FormQuestionId = 0;
        }

        public string ControlType { get; set; }
        public int ControlTypeId { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool IsChecked { get; set; }
        public long FormQuestionId { get; set; }
        public DateTime? DateValue { get; set; }
        public DateTime? StartDateValue { get; set; }
        public DateTime? EndDateValue { get; set; }
        public TimeSpan? TimeValue { get; set; }
        public TimeSpan? StartTimeValue { get; set; }
        public TimeSpan? EndTimeValue { get; set; }
        public DataSourceDto SelectedItem { get; set; }
        public SelectedOptionDto SelectedOption { get; set; }
        public ObservableCollection<DataSourceDto> DataSource { get; set; }
        public BaseQuestion Question { get; set; }
        public string SelectedRadioOption { get; set; }

        private ObservableCollection<object> multiSelectOptions_;

        public ObservableCollection<object> MultiSelectOptions
        {
            get { return multiSelectOptions_; }
            set { multiSelectOptions_ = value; RaisePropertyChanged(() => MultiSelectOptions); }
        }

        //public bool HasError { get; set; }
        private bool hasError_;

        public bool HasError
        {
            get { return hasError_; }
            set { hasError_ = value; RaisePropertyChanged(() => HasError); }
        }
    }
}