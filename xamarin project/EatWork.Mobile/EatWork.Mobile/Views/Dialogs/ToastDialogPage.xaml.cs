using EatWork.Mobile.Models.DataObjects;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ToastDialogPage : PopupPage
    {
        #region Variables

        protected Dictionary<DialogAction, Task> DialogActions;
        protected Action OnApearing;
        protected TaskCompletionSource<object> Proccess;

        //protected ILocator _locator;
        protected IPopupNavigation _popupNavigation;

        private ToastMessageRequest model_;

        #endregion Variables

        public ToastDialogPage(ToastMessageRequest args)
        {
            InitializeComponent();
            _popupNavigation = PopupNavigation.Instance;
            model_ = args;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.CardView.IndicatorColor = model_.Color;
            this.Icon.TextColor = model_.Color;

            if (!string.IsNullOrWhiteSpace(model_.Icon))
            {
                this.Icon.Text = model_.Icon;
            }

            if (!string.IsNullOrWhiteSpace(model_.Title))
            {
                this.Title.Text = model_.Title;
            }

            this.Message.Text = model_.Message;

            OnApearing?.Invoke();
            Proccess = new TaskCompletionSource<object>();
        }

        public virtual Task<object> GetResult()
        {
            return Proccess.Task;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //Proccess.SetResult(false);
            await _popupNavigation.PopAsync(true);
        }
    }
}