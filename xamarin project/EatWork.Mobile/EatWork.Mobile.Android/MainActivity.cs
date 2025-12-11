using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Platform;
using Plugin.LocalNotification;
using Plugin.Media;
using Plugin.Permissions;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace EatWork.Mobile.Droid
{
    [Activity(Label = "Everything at Work", Icon = "@mipmap/appicon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const int RequestNotificationPermissionCode = 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Init(savedInstanceState);

            /*
            //Window.AddFlags(WindowManagerFlags.Fullscreen);
            //Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);

            //TransparentStatusBar();
            */
            LoadApplication(new App());

            this.SetStatusBarColor(Android.Graphics.Color.Argb(255, 0, 0, 0));

            //Add the following code for scrollable views:
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);

            NotificationCenter.NotifyNotificationTapped(Intent);
        }

        private void RequestNotificationPermission()
        {
            // Check if the permission is already granted
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
            {
                // Request the permission
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.PostNotifications }, RequestNotificationPermissionCode);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Notification permission already granted.");
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            NotificationCenter.NotifyNotificationTapped(intent);
            base.OnNewIntent(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
                Task.Run(async () =>
                {
                    await PopupNavigation.Instance.PopAllAsync();
                });
            }
        }

        private async void Init(Bundle savedInstanceState)
        {
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            await CrossMedia.Current.Initialize();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            //Plugin.Iconize.Iconize.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);
            Plugin.Iconize.Iconize.Init(Resource.Id.main_toolbar, Resource.Id.sliding_tabs);

            UserDialogs.Init(this);

            // FFImageLoading library
            CachedImageRenderer.Init(true);

            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();

            //Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            NotificationCenter.CreateNotificationChannel();

            // For Android 13+ (API level 33), request the notification permission at runtime
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Android 13+
            {
                RequestNotificationPermission();
            }

            //InteractiveAlerts.Init(() => this);

            #region preload images

            ImageService.Instance.LoadFile("InfoContent.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("inprogress.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("NoInternet.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("SomethingWentWrong.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("NoItems.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Contactinformation.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Personalinformation.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("FamilyBackground.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("EducationalBackground.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("CharacterReference.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("IdentificationCard.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Resume.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("EmploymentInformation.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("PersonnelDevelopment.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("EmployeeProfile.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Gift.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Awards.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Cancel.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("EmployeeGroup.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Certificate.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();

            ImageService.Instance.LoadFile("Beach.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("BusinessDeal.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("WorkingLate.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("TimeManagement.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("CalendarSvg.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Schedule.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Wallet.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("QuittingTime.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("chilling.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("multitasking.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("accessdenied.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();

            ImageService.Instance.LoadFile("Working.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Letters.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Confirmed.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Confirmed.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Success.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("Error.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("SideMenu.jpg").WithCustomDataResolver().Preload();
            ImageService.Instance.LoadFile("LoginBackground2.jpg").WithCustomDataResolver().Preload();
            ImageService.Instance.LoadFile("HeaderImageProfile.png").WithCustomDataResolver().Preload();
            ImageService.Instance.LoadFile("MailSent.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("OnlinePayment.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("EmptyCart.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();

            ImageService.Instance.LoadFile("welcomebanner.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("emojiangry.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("emojidisappointed.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("emojihappy.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();
            ImageService.Instance.LoadFile("emojiproud.svg").WithCustomDataResolver(new SvgDataResolver(200, 0, true)).Preload();

            #endregion preload images
        }

        private void TransparentStatusBar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                //setWindowFlag(this, WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS, true);
                this.Window.AddFlags(WindowManagerFlags.TranslucentStatus);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                //setWindowFlag(this, WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS, true);
                //if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
                //{
                //    Window.SetDecorFitsSystemWindows(true);
                //}
                //else
                //{
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LayoutFullscreen;
                //Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
                //}

                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
            }

            //if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            //{
            //    //setWindowFlag(this, WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS, true);
            //    Window.AddFlags(WindowManagerFlags.TranslucentStatus);
            //}

            /*
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                // for covering the full screen in android..
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

                // clear FLAG_TRANSLUCENT_STATUS flag:
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

                // add FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS flag to the window
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
                {
                    Window.SetDecorFitsSystemWindows(true);
                }
                else
                {
                    Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
                }

                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
            }
            */
        }
    }
}