using System;
using System.ComponentModel;

namespace EatWork.Mobile.Models.FormHolder.IndividualObjectives
{
    public class IndividualObjectives
    {
        public long IndividualOjbectiveId { get; set; }
        public long ProfileId { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public string Period { get; set; }
        public DateTime DatePrepared { get; set; }
        public int EffectiveYear { get; set; }
        public string Details { get; set; }
        public string Header { get; set; }
        public string DatePrepared_String { get; set; }
        public string Icon { get; set; }
        public string Icon2 { get; set; }
    }

    public class IndividualObjectivesDto : IndividualObjectives, INotifyPropertyChanged
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}