using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmationPage : DialogBase
    {
        public string YesButtonText { get; set; }
        public string NoButtonText { get; set; }

        public ConfirmationPage(string message)
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
                Proccess.SetResult(false);
            };

            this.btnYes.Clicked += (sender, args) =>
            {
                Proccess.SetResult(true);
            };
        }
    }
}