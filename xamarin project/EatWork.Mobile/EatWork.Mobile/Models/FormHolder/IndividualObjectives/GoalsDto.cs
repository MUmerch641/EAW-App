using EatWork.Mobile.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Models.FormHolder.IndividualObjectives
{
    public class GoalsHolder : ExtendedBindableObject
    {
        public GoalsHolder()
        {
            Header = new GoalHeaderDto();
            Year = string.Empty;
            GoalHeaderDetails = new ObservableCollection<GoalHeaderDetailDto>();
        }

        private GoalHeaderDto header_;

        public GoalHeaderDto Header
        {
            get { return header_; }
            set { header_ = value; RaisePropertyChanged(() => Header); }
        }

        private string year_;

        public string Year
        {
            get { return year_; }
            set { year_ = value; RaisePropertyChanged(() => Year); }
        }

        private List<R.Models.ObjectiveDataDto> parentObjectiveList_;

        public List<R.Models.ObjectiveDataDto> ParentObjectiveList
        {
            get { return parentObjectiveList_; }
            set { parentObjectiveList_ = value; RaisePropertyChanged(() => ParentObjectiveList); }
        }

        private List<R.Models.OrganizationGoalDto2> otherObjectiveList_;

        public List<R.Models.OrganizationGoalDto2> OtherObjectiveList
        {
            get { return otherObjectiveList_; }
            set { otherObjectiveList_ = value; RaisePropertyChanged(() => OtherObjectiveList); }
        }

        private ObservableCollection<GoalHeaderDetailDto> goalHeaderDetails_;

        public ObservableCollection<GoalHeaderDetailDto> GoalHeaderDetails
        {
            get { return goalHeaderDetails_; }
            set { goalHeaderDetails_ = value; RaisePropertyChanged(() => GoalHeaderDetails); }
        }
    }

    public class GoalHeaderDto
    {
        public GoalHeaderDto()
        {
        }

        public long HeaderId { get; set; }
        public string GoalName { get; set; }
        public string GoalDescription { get; set; }
    }

    public class GoalHeaderDetailDto
    {
        public GoalHeaderDetailDto()
        {
            GoalDetails = new ObservableCollection<GoalDetailDto>();
        }

        public long HeaderDetailId { get; set; }
        public long HeaderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<GoalDetailDto> GoalDetails { get; set; }
    }

    public class GoalDetailDto
    {
        public long DetailId { get; set; }
        public long HeaderDetailId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HeaderDetailName { get; set; }
    }
}