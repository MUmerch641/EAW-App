using EatWork.Mobile.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WarningPage : ContentPage
    {
        private TaskCompletionSource<bool> _taskCompletionSource;

        public WarningPage(ObservableCollection<string> msg = null, string title = "")
        {
            InitializeComponent();

            _taskCompletionSource = new TaskCompletionSource<bool>();

            Messages.ItemsSource = msg;

            if (!string.IsNullOrWhiteSpace(title))
                Title.Text = title;
        }

        private async void CloseModal_Clicked(object sender, System.EventArgs e)
        {
            PreferenceHelper.WarningPageClosed(true);
            await Navigation.PopModalAsync(true);
            _taskCompletionSource.SetResult(true);
        }

        /*
        protected override void OnDisappearing()
        {
            MessagingCenter.Send(this, "WarningPageClosed");
            PreferenceHelper.WarningPageClosed(true);
            base.OnDisappearing();
        }
        */

        public Task WaitForModalToCloseAsync()
        {
            return _taskCompletionSource.Task;
        }
    }
}