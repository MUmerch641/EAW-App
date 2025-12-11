using SQLite;

namespace EatWork.Mobile.Models.DataAccess
{
    [Table("ClientSetup")]
    public class ClientSetupModel
    {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }

        public long ClientId { get; set; }
        public string ClientCode { get; set; }
        public string Passkey { get; set; }
        public string APILink { get; set; }
        public string LoginScreenImage { get; set; }
        public string HomeScreenImage { get; set; }
        public string LoginScreenImageType { get; set; }
        public string HomeScreenImageType { get; set; }
        public string LogoImage { get; set; }
        public string LogoImageType { get; set; }
        public long? ThemeConfigId { get; set; }
        public string BrandingImage { get; set; }
        public string BrandingImageType { get; set; }
        public string HomePageImage { get; set; }
        public string HomePageImageType { get; set; }
    }
}