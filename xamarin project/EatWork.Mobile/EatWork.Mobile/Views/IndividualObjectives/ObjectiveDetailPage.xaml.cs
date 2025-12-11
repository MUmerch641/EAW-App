using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels.IndividualObjectives;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.IndividualObjectives
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ObjectiveDetailPage : ContentPage
    {
        public EventHandler<bool> OnPageClosed;

        public ObjectiveDetailPage(string year = "", ObjectiveDetailDto item = null)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<ObjectiveDetailViewModel>();
            viewModel.Init(Navigation, year, item);
            /*viewModel.Form(item);*/
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<ObjectiveDetailPage>(this, "onback");
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (IndividualObjectiveHelper.ObjectiveDetailChanged())
            {
                Debug.WriteLine("ObjectiveDetailPage OnPageClosed invoked");
                OnPageClosed.Invoke(this, true);
            }
        }
    }
}