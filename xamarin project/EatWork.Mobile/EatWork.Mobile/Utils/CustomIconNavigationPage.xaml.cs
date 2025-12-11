using Plugin.Iconize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Utils
{
    public partial class CustomIconNavigationPage : IconNavigationPage
    {
        public CustomIconNavigationPage(Page root) : base(root)
        {
            InitializeComponent();
            BarTextColor = Color.White;
        }
    }
}