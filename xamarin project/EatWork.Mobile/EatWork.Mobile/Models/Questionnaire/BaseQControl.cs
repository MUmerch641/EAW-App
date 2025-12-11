using Xamarin.Forms;

namespace EatWork.Mobile.Models.Questionnaire
{
    public class BaseQControl
    {
        public BaseQuestion BaseQuestion { get; set; }

        public bool IsHr { get; set; }

        public string Question { get; set; }

        public string QuestionName { get; set; }

        public string GeneratedQuestion { get; set; }

        public int QuestionNumber { get; set; }

        public string QuestionSection { get; set; }

        public short SectionSortOrder { get; set; }

        public short DisplayPage { get; set; }

        public short QuestionSortOrder { get; set; }
    }

    public class BaseQControlDto : BaseQControl
    {
        public BaseQControlDto()
        {
            HasAnswer = false;  
        }
        public string Name { get; set; }
        public string ID { get; set; }
        public string QuestionHeader { get; set; }
        public string QuestionDetail { get; set; }
        public string ControlName { get; set; }
        public long FormHeaderId { get; set; }
        public long FormSurveyHistoryId { get; set; }
        public View RotatorItem { get; set; }
        public string Answer { get; set; }
        public bool HasAnswer { get; set; }
    }
}