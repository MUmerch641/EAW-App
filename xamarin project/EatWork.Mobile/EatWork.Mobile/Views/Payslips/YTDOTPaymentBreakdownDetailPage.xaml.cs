using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Payslip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Payslips
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YTDOTPaymentBreakdownDetailPage : ContentPage
    {
        public YTDOTPaymentBreakdownDetailPage(long paysheetHeaderId, long profileId)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<YTDOTPaymentBreakdownDetailViewModel>();
            viewModel.Init(Navigation, paysheetHeaderId, profileId);
            BindingContext = viewModel;
        }
    }
}