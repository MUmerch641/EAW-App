using System;

namespace EatWork.Mobile.Models.Workflow
{
    public class TaskListModel
    {
        public DateTime DateTime { get; set; }
        public string Link { get; set; }
        public int RowCount { get; set; }
        public string StageType { get; set; }
        public string Submitter { get; set; }
        public long TransaactionId { get; set; }
        public string TransactionType { get; set; }
    }
}