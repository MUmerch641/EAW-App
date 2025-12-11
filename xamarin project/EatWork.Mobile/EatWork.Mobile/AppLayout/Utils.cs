using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Themes;
using EatWork.Mobile.Utils.DataAccess;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EatWork.Mobile.AppLayout
{
    [Preserve(AllMembers = true)]
    public static class Extensions
    {
        public static void ApplyDarkTheme(this ResourceDictionary resources)
        {
            if (resources != null)
            {
                var mergedDictionaries = resources.MergedDictionaries;
                var lightTheme = mergedDictionaries.OfType<LightTheme>().FirstOrDefault();
                if (lightTheme != null)
                {
                    mergedDictionaries.Remove(lightTheme);
                }

                // mergedDictionaries.Add(new DarkTheme());
                AppSettings.Instance.IsDarkTheme = true;
            }
        }

        public static void ApplyLightTheme(this ResourceDictionary resources)
        {
            if (resources != null)
            {
                var mergedDictionaries = resources.MergedDictionaries;

                // var darkTheme = mergedDictionaries.OfType<DarkTheme>().FirstOrDefault();
                // if (darkTheme != null)
                // {
                //     mergedDictionaries.Remove(darkTheme);
                // }
                mergedDictionaries.Add(new LightTheme());
                AppSettings.Instance.IsDarkTheme = false;
            }
        }

        public static void ApplyColorSet()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var container = AppContainer.Resolve<ThemeDataAccess>();
                var theme = await container.RetrieveThemeSetup();

                if (theme != null)
                {
                    if (!string.IsNullOrWhiteSpace(theme.PrimaryColor))
                        Application.Current.Resources["PrimaryColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryDarkColor))
                        Application.Current.Resources["PrimaryDarkColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryDarkColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryDarkenColor))
                        Application.Current.Resources["PrimaryDarkenColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryDarkenColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryLighterColor))
                        Application.Current.Resources["PrimaryLighterColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryLighterColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryGradient))
                        Application.Current.Resources["PrimaryGradient"] = Xamarin.Forms.Color.FromHex(theme.PrimaryGradient);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryLight))
                        Application.Current.Resources["PrimaryLight"] = Xamarin.Forms.Color.FromHex(theme.PrimaryLight);

                    if (!string.IsNullOrWhiteSpace(theme.SecondaryGradient))
                        Application.Current.Resources["SecondaryGradient"] = Xamarin.Forms.Color.FromHex(theme.SecondaryGradient);

                    if (!string.IsNullOrWhiteSpace(theme.Secondary))
                        Application.Current.Resources["Secondary"] = Xamarin.Forms.Color.FromHex(theme.Secondary);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryButtonColor))
                        Application.Current.Resources["PrimaryButtonColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryButtonColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryButtonGradient))
                        Application.Current.Resources["PrimaryButtonGradient"] = Xamarin.Forms.Color.FromHex(theme.PrimaryButtonGradient);

                    if (!string.IsNullOrWhiteSpace(theme.LoginGradientStart))
                    {
                        if (theme.LoginGradientStart == Constants.TRANSPARENT)
                            Application.Current.Resources["LoginGradientStart"] = Application.Current.Resources["Transparent"];
                        else
                            Application.Current.Resources["LoginGradientStart"] = Xamarin.Forms.Color.FromHex(theme.LoginGradientStart);
                    }

                    if (!string.IsNullOrWhiteSpace(theme.LoginGradientEnd))
                    {
                        if (theme.LoginGradientStart == Constants.TRANSPARENT)
                            Application.Current.Resources["LoginGradientEnd"] = Application.Current.Resources["Transparent"];
                        else
                            Application.Current.Resources["LoginGradientEnd"] = Xamarin.Forms.Color.FromHex(theme.LoginGradientEnd);
                    }

                    if (!string.IsNullOrWhiteSpace(theme.LoginTextColor))
                    {
                        Application.Current.Resources["LoginTextColor"] = Xamarin.Forms.Color.FromHex(theme.LoginTextColor);
                    }

                    /*DASHBOARD*/
                    if (!string.IsNullOrWhiteSpace(theme.DashboardPrimaryTextHeader))
                        Application.Current.Resources["DashboardPrimaryTextHeader"] = Xamarin.Forms.Color.FromHex(theme.DashboardPrimaryTextHeader);

                    if (!string.IsNullOrWhiteSpace(theme.DashboardPrimaryTextValue))
                        Application.Current.Resources["DashboardPrimaryTextValue"] = Xamarin.Forms.Color.FromHex(theme.DashboardPrimaryTextValue);

                    if (!string.IsNullOrWhiteSpace(theme.DashboardSecondaryTextHeader))
                        Application.Current.Resources["DashboardSecondaryTextHeader"] = Xamarin.Forms.Color.FromHex(theme.DashboardSecondaryTextHeader);

                    if (!string.IsNullOrWhiteSpace(theme.DashboardSecondaryTextValue))
                        Application.Current.Resources["DashboardSecondaryTextValue"] = Xamarin.Forms.Color.FromHex(theme.DashboardSecondaryTextValue);

                    if (!string.IsNullOrWhiteSpace(theme.VLColorStart))
                        Application.Current.Resources["VLColorStart"] = Xamarin.Forms.Color.FromHex(theme.VLColorStart);

                    if (!string.IsNullOrWhiteSpace(theme.VLColorEnd))
                        Application.Current.Resources["VLColorEnd"] = Xamarin.Forms.Color.FromHex(theme.VLColorEnd);

                    if (!string.IsNullOrWhiteSpace(theme.SLColorStart))
                        Application.Current.Resources["SLColorStart"] = Xamarin.Forms.Color.FromHex(theme.SLColorStart);

                    if (!string.IsNullOrWhiteSpace(theme.SLColorEnd))
                        Application.Current.Resources["SLColorEnd"] = Xamarin.Forms.Color.FromHex(theme.SLColorEnd);

                    if (!string.IsNullOrWhiteSpace(theme.OTMTDColorStart))
                        Application.Current.Resources["OTMTDColorStart"] = Xamarin.Forms.Color.FromHex(theme.OTMTDColorStart);

                    if (!string.IsNullOrWhiteSpace(theme.OTMTDColorEnd))
                        Application.Current.Resources["OTMTDColorEnd"] = Xamarin.Forms.Color.FromHex(theme.OTMTDColorEnd);

                    if (!string.IsNullOrWhiteSpace(theme.OTYTDColorStart))
                        Application.Current.Resources["OTYTDColorStart"] = Xamarin.Forms.Color.FromHex(theme.OTYTDColorStart);

                    if (!string.IsNullOrWhiteSpace(theme.OTYTDColorEnd))
                        Application.Current.Resources["OTYTDColorEnd"] = Xamarin.Forms.Color.FromHex(theme.OTYTDColorEnd);

                    if (!string.IsNullOrWhiteSpace(theme.AbsencesMTDColor))
                        Application.Current.Resources["AbsencesMTDColor"] = Xamarin.Forms.Color.FromHex(theme.AbsencesMTDColor);

                    if (!string.IsNullOrWhiteSpace(theme.AbsencesYTDColor))
                        Application.Current.Resources["AbsencesYTDColor"] = Xamarin.Forms.Color.FromHex(theme.AbsencesYTDColor);

                    if (!string.IsNullOrWhiteSpace(theme.TardinessMTDColor))
                        Application.Current.Resources["TardinessMTDColor"] = Xamarin.Forms.Color.FromHex(theme.TardinessMTDColor);

                    if (!string.IsNullOrWhiteSpace(theme.TardinessYTDColor))
                        Application.Current.Resources["TardinessYTDColor"] = Xamarin.Forms.Color.FromHex(theme.TardinessYTDColor);

                    if (!string.IsNullOrWhiteSpace(theme.DashboardSecondaryColor))
                        Application.Current.Resources["DashboardSecondaryColor"] = Xamarin.Forms.Color.FromHex(theme.DashboardSecondaryColor);
                }
            });
        }
    }
}