using EatWork.Mobile.Validations;
using System;
using System.Collections.ObjectModel;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Models.FormHolder.IndividualObjectives
{
    public class IndividualObjectiveItemHolder : RequestHolder
    {
        public IndividualObjectiveItemHolder()
        {
            EmployeeName = new ValidatableObject<string>();
            CompanyName = new ValidatableObject<string>();
            DepartmentName = new ValidatableObject<string>();
            Position = new ValidatableObject<string>();
            EffectiveYear = new ValidatableObject<string>();
            Period = new ValidatableObject<long>();
            Status = "New";
            PreparedDate = DateTime.UtcNow.Date;
            MidYearChecked = false;
            AnnualChecked = true;
            Objectives = new ObservableCollection<MainObjectiveDto>();
            ObjectivesLimited = new ObservableCollection<MainObjectiveDto>();
            ObjectivesToSave = new ObservableCollection<ObjectiveDetailDto>();
            IsExceeded = false;
            Model = new R.Models.PerformanceObjectiveHeader();
            IsSaveOnly = false;
            IsEditable = true;
            AddedObjectiveCount = 0;
            ShowButton = true;
        }

        private ValidatableObject<string> employeeName_;

        public ValidatableObject<string> EmployeeName
        {
            get { return employeeName_; }
            set { employeeName_ = value; RaisePropertyChanged(() => EmployeeName); }
        }

        private ValidatableObject<string> companyName_;

        public ValidatableObject<string> CompanyName
        {
            get { return companyName_; }
            set { companyName_ = value; RaisePropertyChanged(() => CompanyName); }
        }

        private ValidatableObject<string> departmentName_;

        public ValidatableObject<string> DepartmentName
        {
            get { return departmentName_; }
            set { departmentName_ = value; RaisePropertyChanged(() => DepartmentName); }
        }

        private ValidatableObject<string> position_;

        public ValidatableObject<string> Position
        {
            get { return position_; }
            set { position_ = value; RaisePropertyChanged(() => Position); }
        }

        private DateTime preparedDate_;

        public DateTime PreparedDate
        {
            get { return preparedDate_; }
            set { preparedDate_ = value; RaisePropertyChanged(() => PreparedDate); }
        }

        private ValidatableObject<string> effectiveYear_;

        public ValidatableObject<string> EffectiveYear
        {
            get { return effectiveYear_; }
            set { effectiveYear_ = value; RaisePropertyChanged(() => EffectiveYear); }
        }

        private ValidatableObject<long> period_;

        public ValidatableObject<long> Period
        {
            get { return period_; }
            set { period_ = value; RaisePropertyChanged(() => Period); }
        }

        private string status_;

        public string Status
        {
            get { return status_; }
            set { status_ = value; RaisePropertyChanged(() => Status); }
        }

        private bool midYearChecked_;

        public bool MidYearChecked
        {
            get { return midYearChecked_; }
            set { midYearChecked_ = value; RaisePropertyChanged(() => MidYearChecked); }
        }

        private bool annualChecked_;

        public bool AnnualChecked
        {
            get { return annualChecked_; }
            set { annualChecked_ = value; RaisePropertyChanged(() => AnnualChecked); }
        }

        /*
        private ObservableCollection<ObjectiveDetailHeaderDto> objectivesLimited_;

        public ObservableCollection<ObjectiveDetailHeaderDto> ObjectivesLimited
        {
            get { return objectivesLimited_; }
            set { objectivesLimited_ = value; RaisePropertyChanged(() => ObjectivesLimited); }
        }

        private ObservableCollection<ObjectiveDetailHeaderDto> objectives_;

        public ObservableCollection<ObjectiveDetailHeaderDto> Objectives
        {
            get { return objectives_; }
            set { objectives_ = value; RaisePropertyChanged(() => Objectives); }
        }
        */

        private ObservableCollection<MainObjectiveDto> objectivesLimited_;

        public ObservableCollection<MainObjectiveDto> ObjectivesLimited
        {
            get { return objectivesLimited_; }
            set { objectivesLimited_ = value; RaisePropertyChanged(() => ObjectivesLimited); }
        }

        private ObservableCollection<MainObjectiveDto> objectives_;

        public ObservableCollection<MainObjectiveDto> Objectives
        {
            get { return objectives_; }
            set { objectives_ = value; RaisePropertyChanged(() => Objectives); }
        }

        private ObservableCollection<ObjectiveDetailDto> objectivesToSave_;

        public ObservableCollection<ObjectiveDetailDto> ObjectivesToSave
        {
            get { return objectivesToSave_; }
            set { objectivesToSave_ = value; RaisePropertyChanged(() => ObjectivesToSave); }
        }

        private bool isExceeded_;

        public bool IsExceeded
        {
            get { return isExceeded_; }
            set { isExceeded_ = value; RaisePropertyChanged(() => IsExceeded); }
        }

        private bool isSaveOnly;

        public bool IsSaveOnly
        {
            get { return isSaveOnly; }
            set { isSaveOnly = value; RaisePropertyChanged(() => IsSaveOnly); }
        }

        private bool isEditable_;

        public bool IsEditable
        {
            get { return isEditable_; }
            set { isEditable_ = value; RaisePropertyChanged(() => IsEditable); }
        }

        private bool showButton_;

        public bool ShowButton
        {
            get { return showButton_; }
            set { showButton_ = value; RaisePropertyChanged(() => ShowButton); }
        }

        private R.Models.PerformanceObjectiveHeader model_;

        public R.Models.PerformanceObjectiveHeader Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        private int addedObjectiveCount_;

        public int AddedObjectiveCount
        {
            get { return addedObjectiveCount_; }
            set { addedObjectiveCount_ = value; RaisePropertyChanged(() => AddedObjectiveCount); }
        }

        public bool IsValid()
        {
            EmployeeName.Validations.Clear();
            EmployeeName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            CompanyName.Validations.Clear();
            CompanyName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            DepartmentName.Validations.Clear();
            DepartmentName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            Position.Validations.Clear();
            Position.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            EffectiveYear.Validations.Clear();
            EffectiveYear.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            Period.Validations.Clear();
            /*
            Period.Validations.Add(new NumberRule<long>
            {
                ValidationMessage = ""
            });
            */

            EmployeeName.Validate();
            CompanyName.Validate();
            DepartmentName.Validate();
            EffectiveYear.Validate();
            Position.Validate();
            Period.Validate();

            if (AddedObjectiveCount == 0)
            {
                EmployeeName.Errors.Add("");
            }

            return EmployeeName.IsValid &&
                   CompanyName.IsValid &&
                   DepartmentName.IsValid &&
                   EffectiveYear.IsValid &&
                   Position.IsValid &&
                   Period.IsValid;
        }
    }
}