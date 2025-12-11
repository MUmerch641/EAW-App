namespace EatWork.Mobile.Models
{
    public class ExpenseSetupModel
    {
        public ExpenseSetupModel()
        {
            ExpenseSetupId = 0;
        }

        public long ExpenseSetupId { get; set; }
        public string ExpenseType { get; set; }
        public string AccountCode { get; set; }
        public string AccountTitle { get; set; }
        public string Icon { get; set; }
        public string IconEquivalent { get; set; }
        public string ColorValue { get; set; }
    }
}