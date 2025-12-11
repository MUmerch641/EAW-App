using EatWork.Mobile.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IDialogService
    {
        Task<bool> AlertAsync(string message, string title = "", string closeText = "Close");

        Task<bool> ConfirmDialogAsync(string message, string title = "", string cofirmText = "Yes", string cancelText = "No");

        Task<InputDialogReponse> InputDialogAsync(string message, string title = "", string cofirmText = "Yes", string cancelText = "No");

        Task<bool> ConfirmDialog2Async(ObservableCollection<string> msg = null, string title = "");

        void ToastMessage(ToastType type, string message);

        Task ToastMessageAsync(ToastType type, string message, string title = "", string closeText = "Close");
        Task ShowLoading();
        void HideLoading();
    }

    public enum ToastType
    {
        Error,
        Warning,
        Success
    }
}