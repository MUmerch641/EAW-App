using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Survey
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveyLandingPage : ContentView
    {
        public SurveyLandingPage(string header = "", string message = "")
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(header))
                HeaderLabel.Text = header;

            if (!string.IsNullOrWhiteSpace(message))
                ContentLabel.Text = message;
        }
    }
}