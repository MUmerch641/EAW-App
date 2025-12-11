namespace EatWork.Mobile.Models
{
    public class AppVersion
    {
        public AppVersion()
        {
            VersionName = "1.0.0.26";
            Android = "1.0.0.26";
            iOS = "1.0.0.26";
            UWP = "1.0.0.26";
            Required = false;
            ForProduction = false;
        }

        public string VersionName { get; set; }
        public string Android { get; set; }
        public string iOS { get; set; }
        public string UWP { get; set; }
        public bool Required { get; set; }
        public bool ForProduction { get; set; }

        public string TestVersionName { get; set; }
        public string TestAndroid { get; set; }
        public string TestiOS { get; set; }
        public string TestUWP { get; set; }
    }
}