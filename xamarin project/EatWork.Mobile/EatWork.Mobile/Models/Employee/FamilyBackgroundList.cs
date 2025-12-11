using System;

namespace EatWork.Mobile.Models
{
    public class FamilyBackground
    {
        public long FamilyBackgroundId { get; set; }
        public long? ProfileId { get; set; }
        public string Relationship { get; set; }
        public string Name { get; set; }
        public DateTime? Birtdate { get; set; }
        public string Occupation { get; set; }
        public string OccAddress { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public short? Dependent { get; set; }
        public string Dependent_JQLabel { get; set; }
        public short? Incapacitated { get; set; }
        public string Incapacitated_JQLabel { get; set; }
        public short? Category { get; set; }
    }
}