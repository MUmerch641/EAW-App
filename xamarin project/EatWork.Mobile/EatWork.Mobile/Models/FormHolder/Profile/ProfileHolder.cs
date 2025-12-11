namespace EatWork.Mobile.Models.FormHolder.Profile
{
    public class ProfileHolder : ProfileModel
    {
        public ProfileHolder()
        {
            CivilStatusString = string.Empty;
            DateOfMarriageString = string.Empty;
            ReligionString = string.Empty;
            BirthdateString = string.Empty;
            NationalityString = string.Empty;
            DualNationalityString = string.Empty;
            MinimumWageEarnerString = string.Empty;
            TaxExemptionStatus = string.Empty;
            SoloParanentString = string.Empty;
            WaiveClaimingOfDependentsString = string.Empty;
            SubstitutedFilingString = string.Empty;
            ApplicableTax = string.Empty;
            FullNameMiddleInitialOnly = string.Empty;
            EmployeeNo = string.Empty;
            Department = string.Empty;
            FullAddress = string.Empty;
            FullProvincialAddress = string.Empty;
            ContactNumber = string.Empty;
            Age = string.Empty;
        }

        public string CivilStatusString { get; set; }
        public string DateOfMarriageString { get; set; }
        public string ReligionString { get; set; }
        public string BirthdateString { get; set; }
        public string NationalityString { get; set; }
        public string DualNationalityString { get; set; }
        public string MinimumWageEarnerString { get; set; }
        public string TaxExemptionStatus { get; set; }
        public string SoloParanentString { get; set; }
        public string WaiveClaimingOfDependentsString { get; set; }
        public string SubstitutedFilingString { get; set; }
        public string ApplicableTax { get; set; }
        public string FullNameMiddleInitialOnly { get; set; }
        public string EmployeeNo { get; set; }
        public string Department { get; set; }
        public string FullAddress { get; set; }
        public string FullProvincialAddress { get; set; }
        public string ContactNumber { get; set; }
        public string Age { get; set; }
    }
}