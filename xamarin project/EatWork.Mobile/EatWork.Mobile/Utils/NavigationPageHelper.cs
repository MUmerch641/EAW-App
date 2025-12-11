using System;
using Xamarin.Forms;

namespace EatWork.Mobile.Utils
{
    public class NavigationPageHelper
    {
        private INavigation navigation_;

        public NavigationPageHelper(INavigation navigation)
        {
            navigation_ = navigation;
        }

        public void PopUntilDestination(Type DestinationPage)
        {
            int LeastFoundIndex = 0;
            int PagesToRemove = 0;

            for (int index = navigation_.NavigationStack.Count - 2; index > 0; index--)
            {
                if (navigation_.NavigationStack[index].GetType().Equals(DestinationPage))
                {
                    break;
                }
                else
                {
                    LeastFoundIndex = index;
                    PagesToRemove++;
                }
            }

            for (int index = 0; index < PagesToRemove; index++)
            {
                navigation_.RemovePage(navigation_.NavigationStack[LeastFoundIndex]);
            }

            navigation_.PopAsync(true);
        }
    }
}