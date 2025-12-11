using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ComingSoonPage : ContentPage
    {
        public ComingSoonPage()
        {
            InitializeComponent();
        }

        public ComingSoonPage(string title = "")
        {
            InitializeComponent();
            Title = title;
        }
    }
}