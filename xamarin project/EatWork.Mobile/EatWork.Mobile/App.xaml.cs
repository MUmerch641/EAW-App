using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Iconize;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: ExportFont("iconize-materialdesignicons.ttf", Alias = "MDIIcons")]

namespace EatWork.Mobile
{
    public partial class App : Application
    {
        /*public IDialogService DialogService;*/

        /*private ICommonDataService commonDataService_;*/

        public App()
        {
            Init();

            InitializeComponent();

            /*MainPage = new CustomIconNavigationPage(new LoginPage());*/
            /*MainPage = new CustomIconNavigationPage(new LandingPage());*/
            MainPage = new NavigationPage(new LandingPage());
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("ios=9d941729-db55-4aba-b130-93884db353de;" +
                              "uwp={Your UWP App secret here};" +
                              "android=d0b6a032-2dd0-4528-96b6-735f21d4e52f",
                              typeof(Analytics), typeof(Crashes));

            AppLayout.Extensions.ApplyColorSet();

            FormSession.IsLoggedIn = false;
            PreferenceHelper.DeviceId(Plugin.DeviceInfo.CrossDeviceInfo.Current.Id);

            var signalR = AppContainer.Resolve<ISignalRDataService>();
            await signalR.StartConnectionAsync();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async void InitSetup()
        {
            /*
            commonDataService_ = AppContainer.Resolve<ICommonDataService>();
            await commonDataService_.ApplyThemeConfig();
            AppLayout.Extensions.ApplyColorSet();
            */
        }

        /*
        private async void InitWalkthrough()
        {
          //RETRIEVE WALKTHROUGH ITEMS ONLOAD APP*
            mainPageDataService_ = AppContainer.Resolve<IMainPageDataService>();
            private var response = await mainPageDataService_.GetBoardingList();
            FormSession.Boardings = FormSession.SetDefaultBoardings(response);
        }
        */

        private void Init()
        {
            AppContainer.RegisterDependencies();

            InitSetup();

            /*InitWalkthrough();*/

            Iconize
                    .With(new Plugin.Iconize.Fonts.MaterialModule())
                    .With(new Plugin.Iconize.Fonts.FontAwesomeBrandsModule())
                    .With(new Plugin.Iconize.Fonts.FontAwesomeRegularModule())
                    .With(new Plugin.Iconize.Fonts.FontAwesomeSolidModule())
                    .With(new Plugin.Iconize.Fonts.MaterialDesignIconsModule())
                    .With(new Plugin.Iconize.Fonts.IoniconsModule());

            /*date format - Jon 11.19.2020*/
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            /*Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzI1ODkyQDMxMzgyZTMzMmUzMFZTcFMybld0ekZJbWphdmF1Tk1BUXN3WjlSUG0rc0dBVzFuNytjR3NMcHc9;MzI1ODkzQDMxMzgyZTMzMmUzMEQ0bmo4K2ZKZXZEYVBPd09JTDJ0dWEyTUMrM3pCcWhhcW5RSnc5eUlRVlU9;MzI1ODk0QDMxMzgyZTMzMmUzMEltMmZQUU5nMkh5emIzU2V3NkRLclM0ZXY5Uk9ZVnluMnFlR3lvUzUyZGM9;MzI1ODk1QDMxMzgyZTMzMmUzMFg2OExBay9QSHh1OTV4ZVBkbjBHcXRjTE4wRk5wOVk3NDFSRWcwS1ZYVWc9;MzI1ODk2QDMxMzgyZTMzMmUzME5HTVp2VFJvUVlCTVk4MmNyUzV3akpvR0p2dmxSZWdVaG1kUktRdUgyUVk9;MzI1ODk3QDMxMzgyZTMzMmUzMERsUDYrOUlsK01TcjAydkhLMElVWEt0dVNvMnBHd01DZTZ0NmROa3F2WmM9;MzI1ODk4QDMxMzgyZTMzMmUzMFgyclRMSU5selNQS3d4bXhJdE14U1ZGWXRWalZyVkZna2l4N1ZLNGQwUjQ9;MzI1ODk5QDMxMzgyZTMzMmUzMEw0bEFQVHozdjRSOE5PT0dKSmxveGNENzdiaSs0ZFdVTzVzWHgxK1JTeUk9;MzI1OTAwQDMxMzgyZTMzMmUzMFVVa3J1Snh3R1VMcTFHbmJ6WXB6eSthaW9JZDRScjNKemg4V2grbHFWYWc9;MzI1OTAxQDMxMzgyZTMzMmUzMGxodGtsd2JSVkxoTjZuVUtzZS92K3dVNXkyQ1NmdWJEQ2JGTlJjbU9ITU09");*/
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHJqVVhjWlpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF9jQX9bdkBnXH1YcHRQTg==;Mgo+DSMBPh8sVXJ0S0R+XE9HcFRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3xTfkRrWXpadXZTRmNeWQ==;ORg4AjUWIQA/Gnt2VVhiQlFadVlJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdk1iUX9bdHFXQGBYVEw=;Njk5ODc4QDMyMzAyZTMyMmUzMGJZTC9MQ0wvd04yWVMwdnJJVkRWMmVTdXJLUjBKSDVFWDE1RnFLZUZMRkU9;Njk5ODc5QDMyMzAyZTMyMmUzMFZIeG9NT0hnVFk2ZzBlTmtpS3h3Nlp3OVNUUGRTNldobHhsdXR0bXlhWUE9;NRAiBiAaIQQuGjN/V0Z+Xk9EaFxEVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdEVrWHdedXBQRGZdUEB+;Njk5ODgxQDMyMzAyZTMyMmUzMFcrSXUzV1gxbXdRMlBvaTF0OWMreUdTUzg5eTZ5VDZFbHhxeDNKZjFROG89;Njk5ODgyQDMyMzAyZTMyMmUzMFA2S01EV3ltNlpJZ1RRSi9MUVpiRzVFa1BheVdraWhRTFQ5NktteWI3SXM9;Mgo+DSMBMAY9C3t2VVhiQlFadVlJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdk1iUX9bdHFXQGFbVUw=;Njk5ODg0QDMyMzAyZTMyMmUzMEpaU1FXdUNzYXZVVzdRMXk4SmhSdUhQWDVOanZhVUl4SGJaQTlHdzlOL1E9;Njk5ODg1QDMyMzAyZTMyMmUzMGc1RG1pZmk2QUF1NmhURUlKbXJEUy9oUit4NVBoTkc0a0xScS9ocWtKbWc9;Njk5ODg2QDMyMzAyZTMyMmUzMFcrSXUzV1gxbXdRMlBvaTF0OWMreUdTUzg5eTZ5VDZFbHhxeDNKZjFROG89");

            /*track version of app*/
            VersionTracking.Track();

            Xamarin.Forms.Device.SetFlags(new[] { "Brush_Experimental" });
        }
    }
}