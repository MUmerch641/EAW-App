using EatWork.Mobile.Utils;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.IndividualObjectives
{
    public class IndividualObjectivesHolder : ExtendedBindableObject
    {
        public IndividualObjectivesHolder()
        {
            ListItemsSource = new ObservableCollection<IndividualObjectivesDto>();
            IsMultipleMode = false;
            SelectedTaskCount = 0;
        }

        private ObservableCollection<IndividualObjectivesDto> listItemsSource_;

        public ObservableCollection<IndividualObjectivesDto> ListItemsSource
        {
            get { return listItemsSource_; }
            set { listItemsSource_ = value; RaisePropertyChanged(() => ListItemsSource); }
        }

        private bool _isMultipleMode;

        public bool IsMultipleMode
        {
            get { return _isMultipleMode; }
            set { _isMultipleMode = value; RaisePropertyChanged(() => IsMultipleMode); }
        }

        private int _selectedTaskCount;

        public int SelectedTaskCount
        {
            get { return _selectedTaskCount; }
            set { _selectedTaskCount = value; RaisePropertyChanged(() => SelectedTaskCount); }
        }
    }
}