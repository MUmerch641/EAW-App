namespace EatWork.Mobile.Models
{
    public class VendorModel
    {
        public VendorModel()
        {
            VendorId = 0;
            Name = string.Empty;
            TINNo = string.Empty;
            Address = string.Empty;
            DisplayPath = string.Empty;
        }

        public long VendorId { get; set; }
        public string Name { get; set; }
        public string TINNo { get; set; }
        public string Address { get; set; }
        public string DisplayPath { get; set; }
        public short? SourceId { get; set; }
    }
}