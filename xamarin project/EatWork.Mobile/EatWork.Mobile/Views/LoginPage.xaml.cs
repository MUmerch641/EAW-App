using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly IDialogService dialogService_;

        public LoginPage(bool HasNavigationpage = false)
        {
            InitializeComponent();

            //NavigationPage.SetHasNavigationBar(this, HasNavigationpage);

            TitleViewBar.IsVisible = HasNavigationpage;
            GradientView.IsVisible = HasNavigationpage;

            if (HasNavigationpage)
            {
                GridContent.Children.Add((View)GradientView, 0, 0);
                GridContent.Children.Add((View)ContentView, 0, 2);
            }
            else
            {
                var content = (View)ContentView;
                GridContent.Children.Add(content, 0, 0);
                Grid.SetRowSpan(content, 3);
            }

            var viewModel = AppContainer.Resolve<LoginViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;

            dialogService_ = AppContainer.Resolve<IDialogService>();

            btnOnlineTimeEntry.IsVisible = !HasNavigationpage;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.ExitApplication))
                {
                    base.OnBackButtonPressed();
                }
            });

            return true;
        }
    }
}