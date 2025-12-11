using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views;
using Syncfusion.SfRotator.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EatWork.Mobile.ViewModels
{
    [Preserve(AllMembers = true)]
    public class WalkthroughViewModel : BaseViewModel
    {
        #region commands

        public ICommand SkipCommand
        {
            protected set;
            get;
        }

        public ICommand NextCommand
        {
            protected set;
            get;
        }

        #endregion commands

        #region properties

        private ObservableCollection<Boarding> boardings;

        public ObservableCollection<Boarding> Boardings
        {
            get
            {
                return boardings;
            }

            set
            {
                if (boardings == value)
                {
                    return;
                }

                boardings = value;
                RaisePropertyChanged(() => Boardings);
            }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }

            set
            {
                if (selectedIndex == value)
                {
                    return;
                }

                selectedIndex = value;
                RaisePropertyChanged(() => SelectedIndex);
            }
        }

        private string nextButtonText = "NEXT";

        public string NextButtonText
        {
            get
            {
                return nextButtonText;
            }

            set
            {
                if (nextButtonText == value)
                {
                    return;
                }

                nextButtonText = value;
                RaisePropertyChanged(() => NextButtonText);
            }
        }

        private bool showSkipButton;

        public bool ShowSkipButton
        {
            get { return showSkipButton; }
            set { showSkipButton = value; RaisePropertyChanged(() => ShowSkipButton); }
        }

        #endregion properties

        private readonly IAuthenticationDataService authenticationDataService_;
        private readonly IMainPageDataService mainPageDataService_;

        public WalkthroughViewModel(IAuthenticationDataService authenticationDataService,
            IMainPageDataService mainPageDataService)
        {
            authenticationDataService_ = authenticationDataService;
            mainPageDataService_ = mainPageDataService;
        }

        public void Init(INavigation navigation)
        {
            IsBusy = false;

            NavigationBack = navigation;

            SkipCommand = new Command(async () => await NavigateToMainPage());
            NextCommand = new Command(Next);

            InitForm();
        }

        private async Task NavigateToMainPage()
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);
                Application.Current.MainPage = new NavigationPage(new ConnectionPage());
                //await NavigationService.PopToRootAsync();
            }
        }

        private async void InitForm()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    //await Task.Delay(500);

                    //if (await authenticationDataService_.HasClientSetup())
                    //{
                    //    Application.Current.MainPage = new CustomIconNavigationPage(new LoginPage());
                    //    await NavigationService.PopToRootAsync();
                    //}

                    Boardings = new ObservableCollection<Boarding>
                    {
                        new Boarding()
                        {
                            ImagePath = "Working.svg",
                            Header = "Let's get started",
                            Content = "",
                            RotatorItem = new WalkthroughItemPage()
                        },
                        /*
                        new Boarding()
                        {
                            ImagePath = "Confirmed.svg",
                            Header = "APPROVALS",
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                            RotatorItem = new WalkthroughItemPage()
                        },
                        new Boarding()
                        {
                            ImagePath = "Letters.svg",
                            Header = "REQUESTS",
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                            RotatorItem = new WalkthroughItemPage()
                        },
                        new Boarding()
                        {
                            ImagePath = "Schedule.svg",
                            Header = "SCHEDULES",
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                            RotatorItem = new WalkthroughItemPage()
                        }
                        */
                    };

                    var response = await mainPageDataService_.GetBoardingList();

                    foreach (var item in response)
                    {
                        Boardings.Add(new Boarding()
                        {
                            ImagePath = item.ImagePath,
                            Header = item.Header,
                            Content = item.Content,
                            RotatorItem = new WalkthroughItemPage()
                        });
                    }

                    // Set bindingcontext to content view.
                    foreach (var boarding in this.Boardings)
                    {
                        boarding.RotatorItem.BindingContext = boarding;
                    }
                }
                catch (Exception ex)
                {
                    ShowPage = false;
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool ValidateAndUpdateSelectedIndex(int itemCount)
        {
            if (SelectedIndex >= itemCount - 1)
            {
                return true;
            }

            SelectedIndex++;
            return false;
        }

        private async void Next(object obj)
        {
            var itemCount = (obj as SfRotator).ItemsSource.Count();
            if (ValidateAndUpdateSelectedIndex(itemCount))
            {
                if (SelectedIndex == itemCount - 1)
                    await NavigateToMainPage();
                else
                    MoveToNextPage();
            }
        }

        private void MoveToNextPage()
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}