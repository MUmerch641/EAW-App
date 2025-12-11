using EatWork.Mobile.Models.DataObjects;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InputDialogPage : DialogBase
    {
        public string YesButtonText { get; set; }
        public string NoButtonText { get; set; }

        public InputDialogPage(string message)
        {
            InitializeComponent();

            _message = message;

            OnApearing = () =>
            {
                this.btnNo.Text = NoButtonText;
                this.btnYes.Text = YesButtonText;
            };
            this.btnNo.Clicked += (sender, args) =>
            {
                var response = new InputDialogReponse()
                {
                    Confirmed = false,
                    ResponseText = txtRemarks.Text
                };

                Proccess.SetResult(response);
            };

            this.btnYes.Clicked += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(txtRemarks.Text))
                {
                    MessageContainer.HasError = true;
                }
                else
                {
                    var response = new InputDialogReponse()
                    {
                        Confirmed = true,
                        ResponseText = txtRemarks.Text
                    };

                    Proccess.SetResult(response);
                }
            };
        }
    }
}