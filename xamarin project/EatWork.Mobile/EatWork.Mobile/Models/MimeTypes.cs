using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models
{
    public class MimeType
    {
        public MimeTypes MimeTypes { get; set; }
    }

    public class MimeTypes
    {
        public ObservableCollection<Android> Android { get; set; }
        public ObservableCollection<iOS> iOS { get; set; }
        public ObservableCollection<UWP> UWP { get; set; }
    }

    public class Android
    {
        public string Type { get; set; }
    }

    public class iOS
    {
        public string Type { get; set; }
    }

    public class UWP
    {
        public string Type { get; set; }
    }
}