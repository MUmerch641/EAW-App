using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.PerformanceEvaluation
{
    public class PEListHolder : ExtendedBindableObject
    {
        public PEListHolder()
        {
            ItemSource = new ObservableCollection<PEListDto>();
        }

        private ObservableCollection<PEListDto> itemSource_;

        public ObservableCollection<PEListDto> ItemSource
        {
            get { return itemSource_; }
            set { itemSource_ = value; RaisePropertyChanged(() => ItemSource); }
        }
    }

    public class PEListModel
    {
        public long RecordId { get; set; }
        public string EvaluationType { get; set; }
        public string Status { get; set; }
        public long StatusId { get; set; }
        public string PeriodCovered { get; set; }
        public string ScheduledDate { get; set; }
        public string DueDate_String { get; set; }
        public long ProfileId { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class PEListDto : PEListModel
    {
    }
}