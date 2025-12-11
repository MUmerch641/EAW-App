using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SuccessPage2 : ContentPage
    {
        public SuccessPage2(string title = "", string content = "", bool autoHide = false, string image = "")
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(title))
                Title.Text = title;

            if (!string.IsNullOrWhiteSpace(content))
                Content.Text = content;

            if (!string.IsNullOrWhiteSpace(image))
            {
                Image.Source = image;
                Image.HeightRequest = 150;
                Image.WidthRequest = 150;
            }

            btnClose.IsVisible = !autoHide;
        }

        private void SfButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync(true);
        }
    }
}