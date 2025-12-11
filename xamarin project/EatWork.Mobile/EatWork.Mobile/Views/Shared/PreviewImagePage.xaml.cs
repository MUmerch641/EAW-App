using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreviewImagePage : ContentPage
    {
        public PreviewImagePage(ImageSource source = null)
        {
            InitializeComponent();

            if (source != null)
            {
                Image.Source = source;
            }
        }

        private async void SkipButton_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PopModalAsync();
        }
    }
}