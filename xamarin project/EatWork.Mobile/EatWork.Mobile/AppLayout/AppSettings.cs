using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EatWork.Mobile.AppLayout
{
    /// <summary>
    /// Application settings
    /// </summary>
    [Preserve(AllMembers = true)]
    public class AppSettings
    {
        private bool enableRTL;

        private bool isDarkTheme;

        static AppSettings()
        {
            Instance = new AppSettings();
        }

        public static AppSettings Instance { get; }

        public bool IsSafeAreaEnabled { get; set; } = false;

        public double SafeAreaHeight { get; set; }
        public Thickness SafeArea { get; set; }

        public bool EnableRTL
        {
            get => this.enableRTL;
            set
            {
                if (value == this.enableRTL)
                {
                    return;
                }

                this.enableRTL = value;
                Application.Current.MainPage.FlowDirection =
                    this.enableRTL ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            }
        }

        public bool IsDarkTheme
        {
            get => this.isDarkTheme;
            set
            {
                if (this.isDarkTheme == value)
                {
                    return;
                }

                this.isDarkTheme = value;
                if (this.isDarkTheme)
                {
                    // Dark Theme
                    Application.Current.Resources.ApplyDarkTheme();
                }
                else
                {
                    // Light Theme
                    Application.Current.Resources.ApplyLightTheme();
                }
            }
        }
    }
}