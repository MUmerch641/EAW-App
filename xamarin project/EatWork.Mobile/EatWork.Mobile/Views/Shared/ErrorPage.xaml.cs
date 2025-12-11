using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage : ContentPage
    {
        public ErrorPage(ObservableCollection<string> msg = null, string title = "")
        {
            InitializeComponent();
            Messages.ItemsSource = msg;

            if (!string.IsNullOrWhiteSpace(title))
                Title.Text = title;
        }

        private void CloseModal_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync(true);
        }
    }
}