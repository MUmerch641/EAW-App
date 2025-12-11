using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels.IndividualObjectives;
using System;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.IndividualObjectives
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RateScaleModalPage : FormModal
    {
        public EventHandler<RateScaleDto> OnPageClosed;
        private RateScaleViewModel _viewModel;

        public RateScaleModalPage(RateScaleDto item = null, bool isEditable = true)
        {
            InitializeComponent();

            _viewModel = AppContainer.Resolve<RateScaleViewModel>();
            _viewModel.Init(item, isEditable);
            BindingContext = _viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_viewModel.Model != null)
                OnPageClosed.Invoke(this, _viewModel.Model);
        }
    }
}