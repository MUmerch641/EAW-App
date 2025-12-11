using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertPage : DialogBase
    {
        public string YesButtonText { get; set; }

        public AlertPage(string message, string header = "Message")
        {
            InitializeComponent();

            _message = message;
            _headerText = header;

            OnApearing = () =>
            {
                this.btnYes.Text = YesButtonText;
            };

            this.btnYes.Clicked += (sender, args) =>
            {
                Proccess.SetResult(false);
            };
        }
    }
}