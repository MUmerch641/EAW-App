using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class BaseViewModel : ExtendedBindableObject
    {
        public ICommand EntryFocusCommand { get; set; }
        public ICommand BackButtonCommand { get; set; }
        public ICommand MenuButtonCommand { get; set; }

        private bool isBusy_ = false;

        public bool IsBusy
        {
            get { return isBusy_; }
            set { SetProperty(ref isBusy_, value); }
        }

        private bool showPage_ = true;

        public bool ShowPage
        {
            get { return showPage_; }
            set { SetProperty(ref showPage_, value); }
        }

        private string responseMessage = string.Empty;

        public string ResponseMessage
        {
            get { return responseMessage; }
            set { SetProperty(ref responseMessage, value); }
        }

        private ObservableCollection<string> msg_;

        public ObservableCollection<string> Message
        {
            get { return msg_; }
            set { SetProperty(ref msg_, value); }
        }

        protected IUserDialogs Dialogs { get; }
        public INavigation NavigationBack { get; set; }
        public INavigationService NavigationService { get; set; }

        public BaseViewModel(IUserDialogs dialogs)
        {
            this.Dialogs = dialogs;
            EntryFocusCommand = new Command<View>(EntryFocus);
            ShowPage = true;
            NavigationService = AppContainer.Resolve<INavigationService>();
        }

        public BaseViewModel()
        {
            this.Dialogs = UserDialogs.Instance;
            Message = new ObservableCollection<string>();
            EntryFocusCommand = new Command<View>(EntryFocus);
            MenuButtonCommand = new Command(ShowMenu);
            BackButtonCommand = new Command(BackItemPage);
            ShowPage = true;
            NavigationService = AppContainer.Resolve<INavigationService>();
        }

        protected virtual async void EntryFocus(View view)
        {
            await Task.Run(() =>
            {
                /*Task.Delay(100);*/
                Device.BeginInvokeOnMainThread(() =>
                {
                    view?.Focus();
                });
            });

            /*await Task.Run(() => view?.Focus());*/
        }

        protected virtual async void BackItemPage()
        {
            await NavigationService.PopPageAsync();
        }

        protected virtual void ShowMenu()
        {
            if (Application.Current.MainPage is FlyoutPage master)
            {
                master.IsPresented = true;
            }
        }

        /*

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        */

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="autoHide"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="msglen">MESSAGE WILL HIDE IN SECONDS</param>
        protected virtual async void Success(bool autoHide = false, string content = "", string title = "", int msglen = 3, string image = "")
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new SuccessPage2(title, content, autoHide, image));
            if (autoHide)
            {
                await Task.Delay(msglen * 1000);

                var lastModalPage = Application.Current.MainPage.Navigation.ModalStack;

                if (lastModalPage.Count >= 1)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="autoHide"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="msglen">MESSAGE WILL HIDE IN SECONDS</param>
        protected virtual async Task SuccessAsync(bool autoHide = false, string content = "", string title = "", int msglen = 3, string image = "")
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new SuccessPage2(title, content, autoHide, image));
            if (autoHide)
            {
                await Task.Delay(msglen * 1000);

                var lastModalPage = Application.Current.MainPage.Navigation.ModalStack;

                if (lastModalPage.Count >= 1)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="autoHide"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="msglen">MESSAGE WILL HIDE IN SECONDS</param>
        protected virtual async void Error(bool autoHide = false, string content = "", string title = "", int msglen = 3, string image = "")
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ErrorPage2(title, content, autoHide, image));
            if (autoHide)
            {
                await Task.Delay(msglen * 1000);

                var lastModalPage = Application.Current.MainPage.Navigation.ModalStack;

                if (lastModalPage.Count >= 1)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="autoHide"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="msglen">MESSAGE WILL HIDE IN SECONDS</param>
        protected virtual async Task ErrorAsync(bool autoHide = false, string content = "", string title = "", int msglen = 3, string image = "")
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ErrorPage2(title, content, autoHide, image));
            if (autoHide)
            {
                await Task.Delay(msglen * 1000);

                var lastModalPage = Application.Current.MainPage.Navigation.ModalStack;

                if (lastModalPage.Count >= 1)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <param name="autoHide"></param>
        /// <param name="msglen">MESSAGE WILL HIDE IN SECONDS</param>
        protected virtual async void Error(ObservableCollection<string> results = null, bool autoHide = false, int msglen = 3, string image = "", string title = "")
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ErrorPage(results, title));
            if (autoHide)
            {
                await Task.Delay(msglen * 1000);

                var lastModalPage = Application.Current.MainPage.Navigation.ModalStack;

                if (lastModalPage.Count >= 1)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
        }

        protected virtual void ToastMessage(string message)
        {
            var config = new ToastConfig(message)
            {
                Position = ToastPosition.Bottom,
                Duration = new TimeSpan(0, 0, 5),
                BackgroundColor = Color.FromHex(Contants.Color.Cancelled)
            };

            Dialogs.Toast(config);
        }
    }
}