using Acr.UserDialogs;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Views.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.Services
{
    public class DialogService : IDialogService
    {
        private readonly IPopupNavigation _popupNavigation;
        private readonly INavigation _navigation;
        protected Page CurrentMainPage => Application.Current.MainPage;

        public DialogService()
        {
            _popupNavigation = PopupNavigation.Instance;
        }

        public async Task<bool> ConfirmDialogAsync(string message, string title = "", string cofirmText = "Yes", string cancelText = "No")
        {
            var confirmationDialog = new ConfirmationPage(message)
            {
                YesButtonText = cofirmText,
                NoButtonText = cancelText
            };

            await _popupNavigation.PushAsync(confirmationDialog);
            var response = await confirmationDialog.GetResult();
            await _popupNavigation.PopAllAsync();

            bool retValue = Convert.ToBoolean(response);
            return retValue;
        }

        public async Task<InputDialogReponse> InputDialogAsync(string message, string title = "", string cofirmText = "Yes", string cancelText = "No")
        {
            var confirmationDialog = new InputDialogPage(message)
            {
                YesButtonText = cofirmText,
                NoButtonText = cancelText
            };

            await _popupNavigation.PushAsync(confirmationDialog);
            var response = await confirmationDialog.GetResult();
            await _popupNavigation.PopAllAsync();

            var retValue = response as InputDialogReponse;

            return retValue;
        }

        public async Task<bool> AlertAsync(string message, string title = "", string closeText = "Close")
        {
            var confirmationDialog = new AlertPage(message, title)
            {
                YesButtonText = closeText
            };

            await _popupNavigation.PushAsync(confirmationDialog);
            var response = await confirmationDialog.GetResult();
            await _popupNavigation.PopAllAsync();

            var retValue = Convert.ToBoolean(response);

            return retValue;
        }

        public async Task<bool> ConfirmDialog2Async(ObservableCollection<string> msg = null, string title = "")
        {
            var confirmationDialog = new EatWork.Mobile.Views.Shared.ConfirmationPage(msg, title);

            await Application.Current.MainPage.Navigation.PushModalAsync(confirmationDialog);
            var response = await confirmationDialog.GetResult();
            await Application.Current.MainPage.Navigation.PopModalAsync(true);

            var retValue = Convert.ToBoolean(response);

            return retValue;
        }

        public void ToastMessage(ToastType type, string message)
        {
            throw new NotImplementedException();
        }

        public async Task ToastMessageAsync(ToastType type, string message, string title = "", string closeText = "Close")
        {
            var color = (Color)Xamarin.Forms.Application.Current.Resources["PrimaryColor"];
            var icon = Xamarin.Forms.Application.Current.Resources["InfoCircleIcon"].ToString();

            switch (type)
            {
                case ToastType.Error:
                    color = (Color)Xamarin.Forms.Application.Current.Resources["Error"];
                    icon = Xamarin.Forms.Application.Current.Resources["CloseCircleIcon"].ToString();
                    break;

                case ToastType.Warning:
                    color = (Color)Xamarin.Forms.Application.Current.Resources["Cancel"];
                    icon = Xamarin.Forms.Application.Current.Resources["ExclamationCircleIcon"].ToString();
                    break;

                case ToastType.Success:
                    color = (Color)Xamarin.Forms.Application.Current.Resources["Success"];
                    icon = Xamarin.Forms.Application.Current.Resources["CheckedCircleIcon"].ToString();
                    break;
            }

            var setup = new ToastMessageRequest()
            {
                Message = message,
                Title = title,
                Color = color,
                Icon = icon,
            };

            var page = new ToastDialogPage(setup);

            await _popupNavigation.PushAsync(page);
            await System.Threading.Tasks.Task.Delay(5000);
            await _popupNavigation.RemovePageAsync(page);
        }

        public async Task ShowLoading()
        {
            await Task.Delay(500);
            UserDialogs.Instance.ShowLoading();
        }

        public void HideLoading()
        {
            UserDialogs.Instance.HideLoading();
        }
    }
}