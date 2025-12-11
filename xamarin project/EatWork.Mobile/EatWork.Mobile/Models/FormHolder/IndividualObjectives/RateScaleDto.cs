namespace EatWork.Mobile.Models.FormHolder.IndividualObjectives
{
    public class RateScaleDto
    {
        public RateScaleDto()
        {
            CriteriaId = 0;
            TempId = 0;
            IsDelete = false;
        }

        public long CriteriaId { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Rating { get; set; }
        public string Criteria { get; set; }
        public string DisplayText1 { get; set; }
        public long TempId { get; set; }
        public bool IsDelete { get; set; }
        public long PODetailId { get; set; }
        public long RetrievedCompKPIId { get; set; }
    }
}