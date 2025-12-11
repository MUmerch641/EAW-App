namespace EatWork.Mobile.Models.DataObjects
{
    public class WorkflowAction
    {
        public string ActionType { get; set; }
        public string ActionMessage { get; set; }
        public long ActionTriggeredId { get; set; }
        public long TransactionId { get; set; }
        public long TransactionTypeId { get; set; }
        public long CurrentStageId { get; set; }
        public string SpecialActions { get; set; }
    }
}