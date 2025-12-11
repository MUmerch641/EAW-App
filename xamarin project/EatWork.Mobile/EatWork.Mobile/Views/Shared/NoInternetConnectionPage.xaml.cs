using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoInternetConnectionPage : ContentPage
    {
        public NoInternetConnectionPage()
        {
            InitializeComponent();
        }

        public NoInternetConnectionPage(string title = "", string msg = "")
        {
            InitializeComponent();
            Title = title;

            if (msg != "")
                lblMessage.Text = msg;
        }
    }
}