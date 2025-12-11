using EatWork.Mobile.AppLayout;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Platform;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.SfCalendar.XForms.iOS;
using Syncfusion.SfNumericTextBox.XForms.iOS;
using Syncfusion.SfPicker.XForms.iOS;
using Syncfusion.SfRotator.XForms.iOS;
using Syncfusion.XForms.iOS.BadgeView;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.XForms.iOS.ComboBox;
using UIKit;
using Xamarin;

namespace EatWork.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            InitPlugins();
            LoadApplication(new App());

            Plugin.LocalNotification.NotificationCenter.AskPermission();

            //return base.FinishedLaunching(app, options);
            var result = base.FinishedLaunching(app, options);

            var safeAreInset = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;

            if (safeAreInset.Top > 0)
            {
                AppSettings.Instance.IsSafeAreaEnabled = true;
                AppSettings.Instance.SafeAreaHeight = safeAreInset.Top;
            }

            return result;
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
            base.WillEnterForeground(uiApplication);
        }

        private void InitPlugins()
        {
            SfButtonRenderer.Init();
            new SfComboBoxRenderer();
            SfListViewRenderer.Init();
            new SfNumericTextBoxRenderer();
            ImageCircleRenderer.Init();
            SfBadgeViewRenderer.Init();
            SfCheckBoxRenderer.Init();
            SfRadioButtonRenderer.Init();
            Syncfusion.XForms.iOS.Cards.SfCardViewRenderer.Init();
            CachedImageRenderer.Init();

            Syncfusion.XForms.iOS.Core.SfAvatarViewRenderer.Init();
            Syncfusion.XForms.iOS.Graphics.SfGradientViewRenderer.Init();

            new Syncfusion.SfAutoComplete.XForms.iOS.SfAutoCompleteRenderer();

            SfPickerRenderer.Init();

            SfSwitchRenderer.Init();
            SfCalendarRenderer.Init();

            new SfBusyIndicatorRenderer();

            new SfRotatorRenderer();

            Syncfusion.XForms.iOS.PopupLayout.SfPopupLayoutRenderer.Init();
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();
            Syncfusion.XForms.iOS.Accordion.SfAccordionRenderer.Init();
            Syncfusion.SfRating.XForms.iOS.SfRatingRenderer.Init();
            Syncfusion.XForms.iOS.ProgressBar.SfLinearProgressBarRenderer.Init();
            Syncfusion.XForms.iOS.MaskedEdit.SfMaskedEditRenderer.Init();
            Syncfusion.XForms.iOS.RichTextEditor.SfRichTextEditorRenderer.Init();
            Syncfusion.XForms.iOS.Expander.SfExpanderRenderer.Init();

            Rg.Plugins.Popup.Popup.Init();

            IQKeyboardManager.SharedManager.Enable = true;

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
    }
}