using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels.IndividualObjectives;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.IndividualObjectives
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoalsListPage : ContentPage
    {
        public EventHandler<bool> OnPageClosed;

        public GoalsListPage(ObjectiveDetailHolder holder = null)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<GoalsListViewModel>();
            viewModel.Init(Navigation, holder);
            BindingContext = viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (IndividualObjectiveHelper.SelectedGoalDetailDtoChanged())
                OnPageClosed.Invoke(this, true);
        }
    }
}