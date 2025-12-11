using EatWork.Mobile.Contracts;
using EatWork.Mobile.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.Services
{
    public class NavigationService : INavigationService
    {
        public NavigationService()
        {
        }

        public async Task PopModalAsync()
        {
            try
            {
                var lastModalPage = Application.Current.MainPage.Navigation.ModalStack;

                if (lastModalPage.Count >= 1)
                {
                    if (Application.Current.MainPage is MainFlyoutPage flyoutNav)
                    {
                        await flyoutNav.Detail.Navigation.PopModalAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.Navigation.PopModalAsync();
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task PopPageAsync()
        {
            try
            {
                if (Application.Current.MainPage is MainFlyoutPage flyoutNav)
                {
                    await flyoutNav.Detail.Navigation.PopAsync();
                }
                else if (Application.Current.MainPage is NavigationPage navPage)
                {
                    await navPage.PopAsync();
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task PopToRootAsync()
        {
            try
            {
                if (Application.Current.MainPage is MainFlyoutPage flyoutNav)
                {
                    await flyoutNav.Detail.Navigation.PopToRootAsync();
                }
                else if (Application.Current.MainPage is NavigationPage navPage)
                {
                    await navPage.PopToRootAsync();
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task PushModalAsync(Page page)
        {
            try
            {
                if (Application.Current.MainPage is MainFlyoutPage flyoutNav)
                {
                    await flyoutNav.Detail.Navigation.PushModalAsync(page);
                }
                else
                {
                    await Application.Current.MainPage.Navigation.PushModalAsync(page);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task PushPageAsync(Page page)
        {
            try
            {
                if (Application.Current.MainPage is MainFlyoutPage flyoutNav)
                {
                    await flyoutNav.Detail.Navigation.PushAsync(page);
                }
                else if (Application.Current.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(page);
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(page);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}