using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EatWork.Mobile.Models
{
    [Preserve(AllMembers = true)]
    public class Boarding
    {
        public string ImagePath { get; set; }

        public string Header { get; set; }

        public string Content { get; set; }

        public View RotatorItem { get; set; }
    }

    public class Boardings
    {
        public ObservableCollection<Boarding> Boarding { get; set; }
    }
}