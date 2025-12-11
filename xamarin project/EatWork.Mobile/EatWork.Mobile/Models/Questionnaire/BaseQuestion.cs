namespace EatWork.Mobile.Models.Questionnaire
{
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
}