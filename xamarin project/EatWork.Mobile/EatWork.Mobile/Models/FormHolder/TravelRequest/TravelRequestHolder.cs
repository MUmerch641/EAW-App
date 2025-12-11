using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Validations;
using System;
using System.Collections.ObjectModel;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Models.FormHolder
{
    public class TravelRequestHolder : RequestHolder
    {
        public TravelRequestHolder()
        {
            IsEnabled = true;
            TripType = new ValidatableObject<string>();
            TripTypes = new ObservableCollection<ComboBoxObject>();
            SelectedTripType = new ComboBoxObject();
            Details = new ValidatableObject<string>();
            FirstOrigin = new ValidatableObject<string>();
            FirstDestination = new ValidatableObject<string>();
            SecondOrigin = new ValidatableObject<string>();
            SecondDestination = new ValidatableObject<string>();
            Model = new R.Models.TravelRequest();
            DateRequested = DateTime.Now.Date;
            FirstDepartureDateStr = new ValidatableObject<string>();
            FirstDepartureTimeStr = new ValidatableObject<string>();
            SecondDepartureDateStr = new ValidatableObject<string>();
            SecondDepartureTimeStr = new ValidatableObject<string>();
            Msg = string.Empty;
            SpecialRequestNote = string.Empty;
            ShowDeleteNote = false;
            UploadedFilesDisplay = new ObservableCollection<FileUploadResponse>();
            SelectedFile = new FileUploadResponse();
            Origins = new ObservableCollection<ComboBoxObject>();
            Destinations = new ObservableCollection<ComboBoxObject>();
            EnabledDetailField = false;
            ModuleFormId = ModuleForms.TravelRequest;
        }

        private DateTime dateRequested_;

        public DateTime DateRequested
        {
            get { return dateRequested_; }
            set { dateRequested_ = value; RaisePropertyChanged(() => DateRequested); }
        }

        private string requestedBy_;

        public string RequestedBy
        {
            get { return requestedBy_; }
            set { requestedBy_ = value; RaisePropertyChanged(() => RequestedBy); }
        }

        private ValidatableObject<string> tripType_;

        public ValidatableObject<string> TripType
        {
            get { return tripType_; }
            set { tripType_ = value; RaisePropertyChanged(() => TripType); }
        }

        private ObservableCollection<ComboBoxObject> tripTypes_;

        public ObservableCollection<ComboBoxObject> TripTypes
        {
            get { return tripTypes_; }
            set { tripTypes_ = value; RaisePropertyChanged(() => TripTypes); }
        }

        private ComboBoxObject selectedTripType_;

        public ComboBoxObject SelectedTripType
        {
            get { return selectedTripType_; }
            set { selectedTripType_ = value; RaisePropertyChanged(() => SelectedTripType); }
        }

        private ValidatableObject<string> details_;

        public ValidatableObject<string> Details
        {
            get { return details_; }
            set { details_ = value; RaisePropertyChanged(() => Details); }
        }

        private ValidatableObject<string> firstOrigin_;

        public ValidatableObject<string> FirstOrigin
        {
            get { return firstOrigin_; }
            set { firstOrigin_ = value; RaisePropertyChanged(() => FirstOrigin); }
        }

        private ValidatableObject<string> firstDestination_;

        public ValidatableObject<string> FirstDestination
        {
            get { return firstDestination_; }
            set { firstDestination_ = value; RaisePropertyChanged(() => FirstDestination); }
        }

        private DateTime? firstDepartureDate_;

        public DateTime? FirstDepartureDate
        {
            get { return firstDepartureDate_; }
            set { firstDepartureDate_ = value; RaisePropertyChanged(() => FirstDepartureDate); }
        }

        private TimeSpan? firstDepartureTime_;

        public TimeSpan? FirstDepartureTime
        {
            get { return firstDepartureTime_; }
            set { firstDepartureTime_ = value; RaisePropertyChanged(() => FirstDepartureTime); }
        }

        private ValidatableObject<string> secondOrigin_;

        public ValidatableObject<string> SecondOrigin
        {
            get { return secondOrigin_; }
            set { secondOrigin_ = value; RaisePropertyChanged(() => SecondOrigin); }
        }

        private ValidatableObject<string> secondDestination_;

        public ValidatableObject<string> SecondDestination
        {
            get { return secondDestination_; }
            set { secondDestination_ = value; RaisePropertyChanged(() => SecondDestination); }
        }

        private DateTime? secondDepartureDate_;

        public DateTime? SecondDepartureDate
        {
            get { return secondDepartureDate_; }
            set { secondDepartureDate_ = value; RaisePropertyChanged(() => SecondDepartureDate); }
        }

        private TimeSpan? secondDepartureTime_;

        public TimeSpan? SecondDepartureTime
        {
            get { return secondDepartureTime_; }
            set { secondDepartureTime_ = value; RaisePropertyChanged(() => SecondDepartureTime); }
        }

        private ValidatableObject<string> firstDepartureDateStr_;

        public ValidatableObject<string> FirstDepartureDateStr
        {
            get { return firstDepartureDateStr_; }
            set { firstDepartureDateStr_ = value; RaisePropertyChanged(() => FirstDepartureDateStr); }
        }

        private ValidatableObject<string> firstDepartureTimeStr_;

        public ValidatableObject<string> FirstDepartureTimeStr
        {
            get { return firstDepartureTimeStr_; }
            set { firstDepartureTimeStr_ = value; RaisePropertyChanged(() => FirstDepartureTimeStr); }
        }

        private ValidatableObject<string> secondDepartureDateStr_;

        public ValidatableObject<string> SecondDepartureDateStr
        {
            get { return secondDepartureDateStr_; }
            set { secondDepartureDateStr_ = value; RaisePropertyChanged(() => SecondDepartureDateStr); }
        }

        private ValidatableObject<string> secondDepartureTimeStr_;

        public ValidatableObject<string> SecondDepartureTimeStr
        {
            get { return secondDepartureTimeStr_; }
            set { secondDepartureTimeStr_ = value; RaisePropertyChanged(() => SecondDepartureTimeStr); }
        }

        private long profileId_;

        public long ProfileId
        {
            get { return profileId_; }
            set { profileId_ = value; RaisePropertyChanged(() => ProfileId); }
        }

        private string specialRequestNote_;

        public string SpecialRequestNote
        {
            get { return specialRequestNote_; }
            set { specialRequestNote_ = value; RaisePropertyChanged(() => SpecialRequestNote); }
        }

        private bool showDeleteNote_;

        public bool ShowDeleteNote
        {
            get { return showDeleteNote_; }
            set { showDeleteNote_ = value; RaisePropertyChanged(() => ShowDeleteNote); }
        }

        private bool enabledDetailField_;

        public bool EnabledDetailField
        {
            get { return enabledDetailField_; }
            set { enabledDetailField_ = value; RaisePropertyChanged(() => EnabledDetailField); }
        }

        private ObservableCollection<ComboBoxObject> origins_;

        public ObservableCollection<ComboBoxObject> Origins
        {
            get { return origins_; }
            set { origins_ = value; RaisePropertyChanged(() => Origins); }
        }

        private ObservableCollection<ComboBoxObject> destinations_;

        public ObservableCollection<ComboBoxObject> Destinations
        {
            get { return destinations_; }
            set { destinations_ = value; RaisePropertyChanged(() => Destinations); }
        }

        private ObservableCollection<FileUploadResponse> uploadedFilesDisplay_;

        public ObservableCollection<FileUploadResponse> UploadedFilesDisplay
        {
            get { return uploadedFilesDisplay_; }
            set { uploadedFilesDisplay_ = value; RaisePropertyChanged(() => UploadedFilesDisplay); }
        }

        private FileUploadResponse selectedFile_;

        public FileUploadResponse SelectedFile
        {
            get { return selectedFile_; }
            set { selectedFile_ = value; RaisePropertyChanged(() => SelectedFile); }
        }

        private long moduleFormId_;

        public long ModuleFormId
        {
            get { return moduleFormId_; }
            set { moduleFormId_ = value; RaisePropertyChanged(() => ModuleFormId); }
        }

        private R.Models.TravelRequest model_;

        public R.Models.TravelRequest Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        public bool ExecuteSubmit()
        {
            Model = new R.Models.TravelRequest()
            {
                ProfileId = ProfileId,
                Details = Details.Value,
                FirstDepartureDate = FirstDepartureDate.GetValueOrDefault(Constants.NullDate),
                FirstDepartureTime = Convert.ToDateTime(Constants.NullDate + FirstDepartureTime.GetValueOrDefault()),
                FirstDestination = FirstDestination.Value,
                Reason = SpecialRequestNote,
                SecondDepartureDate = SecondDepartureDate.GetValueOrDefault(Constants.NullDate),
                SecondDepartureTime = Convert.ToDateTime(Constants.NullDate + SecondDepartureTime.GetValueOrDefault()),
                SecondDestination = SecondDestination.Value,
                RequestDate = DateRequested,
                FirstOrigin = FirstOrigin.Value,
                SecondOrigin = SecondOrigin.Value,
                StatusId = RequestStatusValue.Submitted,
                TypeOfBusinessTrip = SelectedTripType.Id,
                SourceId = (short)SourceEnum.Mobile,
            };

            return IsValid();
        }

        public bool IsValid()
        {
            Details.Validations.Clear();
            /*
            Details.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            */

            FirstOrigin.Validations.Clear();
            FirstOrigin.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            FirstDestination.Validations.Clear();
            FirstDestination.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            SecondOrigin.Validations.Clear();
            SecondOrigin.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            SecondDestination.Validations.Clear();
            SecondDestination.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            FirstDepartureDateStr.Validations.Clear();
            /*
            FirstDepartureDateStr.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            */

            FirstDepartureTimeStr.Validations.Clear();
            /*
            FirstDepartureTimeStr.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            */

            SecondDepartureDateStr.Validations.Clear();
            /*
            SecondDepartureDateStr.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            */

            SecondDepartureTimeStr.Validations.Clear();
            /*
            SecondDepartureTimeStr.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            */

            TripType.Validations.Clear();
            /*
            TripType.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            */

            FirstDestination.Validate();
            FirstOrigin.Validate();
            Details.Validate();
            SecondOrigin.Validate();
            SecondDestination.Validate();
            TripType.Validate();
            FirstDepartureDateStr.Validate();
            FirstDepartureTimeStr.Validate();
            SecondDepartureDateStr.Validate();
            SecondDepartureTimeStr.Validate();

            if (!FirstDepartureDate.HasValue)
                FirstDepartureDateStr.Errors.Add("");

            if (!FirstDepartureTime.HasValue)
                FirstDepartureTimeStr.Errors.Add("");

            if (!SecondDepartureDate.HasValue)
                SecondDepartureDateStr.Errors.Add("");

            if (!SecondDepartureTime.HasValue)
                SecondDepartureTimeStr.Errors.Add("");

            if (string.IsNullOrWhiteSpace(SelectedTripType.Value))
                TripType.Errors.Add("");

            if (SelectedTripType.Id == 1 && string.IsNullOrWhiteSpace(Details.Value))
                Details.Errors.Add("");

            return Details.IsValid
                 && FirstOrigin.IsValid
                 && FirstDestination.IsValid
                 && SecondOrigin.IsValid
                 && SecondDestination.IsValid
                 && FirstDepartureDateStr.IsValid
                 && FirstDepartureTimeStr.IsValid
                 && SecondDepartureDateStr.IsValid
                 && SecondDepartureTimeStr.IsValid
                 && TripType.IsValid;
        }
    }
}