using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class AgendaPageViewModel : BaseViewModel
    {
        public AgendaPageViewModel()
        {
        }

        public void Init(INavigation navigation)
        {
            NavigationBack = navigation;
        }
    }
}