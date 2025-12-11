namespace EatWork.Mobile.Models
{
    public class EducationalBackgroundModel
    {
        public long EducationalBackgroundId { get; set; }
        public long? ProfileId { get; set; }
        public long? EducationalLevel { get; set; }
        public string Dates { get; set; }
        public string HonorCitationsAwards { get; set; }
        public long? School { get; set; }
        public long? Course { get; set; }
        public long? Attainment { get; set; }
    }
}