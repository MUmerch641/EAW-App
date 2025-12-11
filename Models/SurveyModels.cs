using System.Collections.ObjectModel;
using MauiHybridApp.Utils;

namespace MauiHybridApp.Models.Questionnaire;

public class PulseSurveyList
{
    public long FormHeaderId { get; set; }
    public long FormSurveyHistoryId { get; set; }
    public string FormName { get; set; }
    public string FormDescription { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? DateEnd { get; set; }
    public bool IsAnonymous { get; set; }
    public bool IsRequired { get; set; }
    public string Status { get; set; }
    public long AnswerId { get; set; }
}

public class BaseQuestion
{
    public long FormQuestionId { get; set; }
    public string Section { get; set; }
    public short SectionSortOrder { get; set; }
    public short DisplayPage { get; set; }
    public short QuestionSortOrder { get; set; }
    public short ControlTypeId { get; set; }
    public short MaxLength { get; set; }
    public bool IsRequired { get; set; }
    public string Required { get; set; }
    public string Question { get; set; }
    public string Options { get; set; }
    public bool IsRequiredComment { get; set; }
    public string IfAnswer { get; set; }
    public string Comment { get; set; }
    public string QuestionName { get; set; }
}

public class BaseQControlDto
{
    public BaseQControlDto()
    {
        HasAnswer = false;
        BaseQuestion = new BaseQuestion();
    }

    public BaseQuestion BaseQuestion { get; set; }
    public string Name { get; set; }
    public string ID { get; set; }
    public string QuestionHeader { get; set; }
    public string QuestionDetail { get; set; }
    public string ControlName { get; set; }
    public long FormHeaderId { get; set; }
    public long FormSurveyHistoryId { get; set; }
    public string Answer { get; set; }
    public bool HasAnswer { get; set; }
    
    // UI Properties for Binding
    public string Value { get; set; }
    public bool IsChecked { get; set; }
    public DateTime? DateValue { get; set; }
    public DateTime? StartDateValue { get; set; }
    public DateTime? EndDateValue { get; set; }
    public TimeSpan? TimeValue { get; set; }
    public TimeSpan? StartTimeValue { get; set; }
    public TimeSpan? EndTimeValue { get; set; }
    public DataSourceDto SelectedItem { get; set; } = new();
    public ObservableCollection<DataSourceDto> MultiSelectOptions { get; set; } = new();
    
    // Helper for rendering
    public int QuestionNumber { get; set; }
    public string QuestionSection { get; set; }
    public short SectionSortOrder { get; set; }
    public short DisplayPage { get; set; }
    public short QuestionSortOrder { get; set; }
}

public class SurveyHolder
{
    public SurveyHolder()
    {
        ControlList = new ObservableCollection<BaseQControlDto>();
        Answers = new ObservableCollection<AnswerDto>();
    }

    public ObservableCollection<BaseQControlDto> ControlList { get; set; }
    public ObservableCollection<AnswerDto> Answers { get; set; }
    
    public long FormHeaderId { get; set; }
    public long FormSurveyHistoryId { get; set; }
    public bool SubmitAnonimously { get; set; }
    public bool IsSuccess { get; set; }
    public string StartMessage { get; set; }
    public string EndMessage { get; set; }
    public bool HasAnswer { get; set; }
    public bool IsRequired { get; set; }
}

public class AnswerDto
{
    public long FormQuestionId { get; set; }
    public string Value { get; set; }
    public string Comment { get; set; }
    public string Points { get; set; }
}

public class DataSourceDto
{
    public int ID { get; set; }
    public string DisplayText { get; set; }
}

public class SubmitSurveyAnswerRequest
{
    public long FormHeaderId { get; set; }
    public long FormSurveyHistoryId { get; set; }
    public List<AnswerDto> AnswerList { get; set; }
    public bool IsSubmitAnonymously { get; set; }
}

public class UserQuestionnaireFormResponse
{
    public List<PulseSurveyList> SurveyList { get; set; }
}

public class QuestionnaireFormResponse
{
    public long FormHeaderId { get; set; }
    public long FormSurveyHistoryId { get; set; }
    public List<BaseQControlDto> ControlList { get; set; }
    public List<AnswerDto> AnswerList { get; set; }
}

public class GetSurveyChartResponse
{
    public SurveyChartModel BarChart { get; set; }
}

public class SurveyChartModel
{
    public string AnswerDetailsFormQuestionIds { get; set; }
    public string Questions { get; set; }
    public string EndDates { get; set; }
}

public class SurveyChartHolder
{
    public string ChartTitle { get; set; }
    public string PrimaryAxisTitle { get; set; }
    // Add chart data models here later
}
