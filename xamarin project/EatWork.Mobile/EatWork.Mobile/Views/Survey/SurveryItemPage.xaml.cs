using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Survey
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveryItemPage : ContentView
    {
        public SurveryItemPage(View view = null)
        {
            InitializeComponent();

            if (view != null)
            {
                Container.Children.Add(view);
            }
        }
    }
}