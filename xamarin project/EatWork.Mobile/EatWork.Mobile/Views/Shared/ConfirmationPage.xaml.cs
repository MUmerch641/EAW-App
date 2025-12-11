using EatWork.Mobile.Models.DataObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmationPage : ContentPage
    {
        protected Dictionary<DialogAction, Task> DialogActions;
        protected Action OnApearing;
        protected TaskCompletionSource<object> Proccess;

        public ConfirmationPage(ObservableCollection<string> msg = null, string title = "")
        {
            InitializeComponent();
            Messages.ItemsSource = msg;

            if (!string.IsNullOrWhiteSpace(title))
                Title.Text = title;

            btnClose.Clicked += (sender, args) =>
            {
                Proccess.SetResult(false);
            };

            btnContinue.Clicked += (sender, args) =>
            {
                Proccess.SetResult(true);
            };
        }

        //private void CloseModal_Clicked(object sender, System.EventArgs e)
        //{
        //    NavigationBack.PopModalAsync(true);
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            OnApearing?.Invoke();
            Proccess = new TaskCompletionSource<object>();
        }

        public virtual Task<object> GetResult()
        {
            return Proccess.Task;
        }
    }
}