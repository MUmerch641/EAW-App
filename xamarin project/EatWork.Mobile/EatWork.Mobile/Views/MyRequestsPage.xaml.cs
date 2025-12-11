using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyRequestsPage : ContentPage
    {
        /*private MyRequestViewModel viewModel;
        //private int lastItemIndex = 10;*/

        public MyRequestsPage()
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<MyRequestViewModel>();
            viewModel.Init(MyRequestListView, Navigation);
            BindingContext = viewModel;
            /*
            //MyRequestListView.ItemAppearing += MyRequestListView_ItemAppearing;
            //MyRequestListView.ItemAppearing += ListView_ItemAppearing;
            //MyRequestListView.ItemDisappearing += ListView_ItemDisappearing;
            */
        }

        /*
        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.CheckListUpdate();
        }
        */

        /*

        private void MyRequestListView_ItemAppearing(object sender, Syncfusion.ListView.XForms.ItemAppearingEventArgs e)
        {
            var item = (e.ItemData as MyRequestListModel);
            int currentItemIndex = MyRequestListView.DataSource.DisplayItems.IndexOf(item);

            if (currentItemIndex > lastItemIndex)
            {
                GridHeaderContent.TranslateTo(0, -100);
                GridListView.TranslateTo(0, -100);
            }
            else
            {
                GridHeaderContent.TranslateTo(0, 0);
                GridListView.TranslateTo(0, 0);
            }
        }
        */
    }
}