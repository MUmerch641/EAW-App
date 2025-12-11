namespace EatWork.Mobile.Models.DataObjects
{
    public class ConfirmDialogResponse
    {
        public bool Confirmed { get; set; }
    }

    public class InputDialogReponse
    {
        public bool Confirmed { get; set; }
        public string ResponseText { get; set; }
    }

    public class ToastMessageRequest
    {
        public Xamarin.Forms.Color Color { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public enum DialogAction
    {
        Ok,
        Yes,
        No,
        TryAgain,
        Cancel
    }
}