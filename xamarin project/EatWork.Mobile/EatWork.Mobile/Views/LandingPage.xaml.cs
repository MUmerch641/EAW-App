using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingPage : ContentPage
    {
        private readonly ClientSetupDataAccess clientSetup_;

        public LandingPage()
        {
            InitializeComponent();

            clientSetup_ = AppContainer.Resolve<ClientSetupDataAccess>();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var setup = await clientSetup_.RetrieveClientSetup();

            if (setup != null)
            {
                if (!string.IsNullOrWhiteSpace(setup.HomePageImage))
                {
                    var type = (string.IsNullOrWhiteSpace(setup.HomePageImageType) ? "jpeg" : setup.HomePageImageType);
                    var url = new UriBuilder(ApiConstants.BaseApiUrl)
                    {
                        Path = string.Format(ApiConstants.GetImageSetup, type, setup.HomePageImage)
                    };

                    PreferenceHelper.SplashScreenSetup(url.ToString());
                }
            }

            if (!string.IsNullOrWhiteSpace(PreferenceHelper.SplashScreenSetup()))
            {
                bgColor.IsVisible = true;
                imgLogo.Opacity = 0;
                imgLogo.IsVisible = true;
                imgLogo.Source = PreferenceHelper.SplashScreenSetup();

                await Task.WhenAny<bool>
                (
                    imgLogo.FadeTo(1, 5000)
                );
            }
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}